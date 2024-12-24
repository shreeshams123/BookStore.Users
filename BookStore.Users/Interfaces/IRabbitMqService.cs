namespace BookStore.Users.Interfaces
{
    public interface IRabbitMqService
    {

        void SendMessage(string message);
        void StartConsuming(string queueName, CancellationToken cancellationToken);
    }
}
