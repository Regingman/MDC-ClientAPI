using System;
namespace MyDataCoin.Interfaces
{
    public interface INotification
    {
        void MakeAllRead(Guid id);
    }
}
