namespace MassTransitSend
{
    public interface ISender
    {
        Task Execute(int count);
    }
}