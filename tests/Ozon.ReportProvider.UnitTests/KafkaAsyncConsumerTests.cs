using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Ozon.ReportProvider.Kafka;
using Ozon.ReportProvider.Kafka.Config;
using Polly;

namespace Ozon.ReportProvider.UnitTests;

public class KafkaAsyncConsumerTests
{
    private readonly Mock<IConsumer<Ignore, int>> _consumerFake = new();
    private readonly Mock<IHandler<Ignore, int>> _handlerFake = new();
    private readonly Mock<ILogger<KafkaAsyncConsumer<Ignore, int>>> _loggerFake = new();
    private readonly Mock<IOptions<KafkaSettings>> _optionsFake = new();
    private readonly KafkaAsyncConsumer<Ignore, int> _kafkaAsyncConsumer;

    public KafkaAsyncConsumerTests()
    {
        _optionsFake.Setup(x => x.Value).Returns(new KafkaSettings
        {
            BootstrapServers = "test-server",
            GroupId = "test-group",
            ChannelCapacity = 10,
            BufferDelay = 1,
            Topic = "test-topic"
        });

        var policy = Policy.NoOpAsync();

        _kafkaAsyncConsumer = new KafkaAsyncConsumer<Ignore, int>(
            _optionsFake.Object,
            _loggerFake.Object,
            _handlerFake.Object,
            _consumerFake.Object,
            policy
        );
    }
    
    [Fact]
    public async Task Consume_ShouldHandleConsumedMessages()
    {
        // Arrange
        var consumeResult = new ConsumeResult<Ignore, int>
        {
            Message = new Message<Ignore, int>
            {
                Value = 42
            }
        };
        const int expectedCount = 10;
        var counter = expectedCount;

        _consumerFake.Setup(x => x.Consume(It.IsAny<CancellationToken>()))
            .Returns(() =>
            {
                if (counter-- > 0)
                {
                    return consumeResult;
                }

                return default!;
            });

        // Act
        await _kafkaAsyncConsumer.Consume(default);

        // Assert
        _handlerFake.Verify(x => x.Handle(It.IsAny<IReadOnlyList<ConsumeResult<Ignore, int>>>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task Consume_BatchLessThanBufferCapacity_ShouldConsumeBatchOfMessages()
    {
        // Arrange
        var consumeResult = new ConsumeResult<Ignore, int>
        {
            Message = new Message<Ignore, int>
            {
                Value = 42
            }
        };
        const int expectedCount = 5;
        var counter = expectedCount;

        _consumerFake.Setup(x => x.Consume(It.IsAny<CancellationToken>()))
            .Returns(() =>
            {
                if (counter-- > 0)
                {
                    return consumeResult;
                }

                return default!;
            });

        // Act
        await _kafkaAsyncConsumer.Consume(default);

        // Assert
        _handlerFake.Verify(x => x.Handle(It.IsAny<IReadOnlyList<ConsumeResult<Ignore, int>>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Consume_MultipleBatches_ShouldHandleMultipleTimes()
    {
        // Arrange
        var consumeResult = new ConsumeResult<Ignore, int>
        {
            Message = new Message<Ignore, int>
            {
                Value = 42
            }
        };
        const int expectedCount = 15;
        var counter = expectedCount;

        _consumerFake.Setup(x => x.Consume(It.IsAny<CancellationToken>()))
            .Returns(() =>
            {
                if (counter-- > 0)
                {
                    return consumeResult;
                }

                return default!;
            });

        // Act
        await _kafkaAsyncConsumer.Consume(default);

        // Assert
        _handlerFake.Verify(x => x.Handle(It.IsAny<IReadOnlyList<ConsumeResult<Ignore, int>>>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }
}