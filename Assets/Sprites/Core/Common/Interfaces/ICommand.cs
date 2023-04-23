using UnityEngine;
using System.Collections;
namespace BaseFrame
{
    public interface ICommand
    {
        void Execute(IMessage message);
    }

}