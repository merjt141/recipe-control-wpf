using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Services.Interfaces
{
    public interface IScaleCommunicationService
    {
        /// <summary>
        /// Connect to the scale
        /// </summary>
        /// <returns></returns>
        Task ConnectAsync();

        /// <summary>
        /// Disconnect from the scale
        /// </summary>
        /// <returns></returns>
        Task DisconnectAsync();

        /// <summary>
        /// Retrieve if the scale is online
        /// </summary>
        /// <returns></returns>
        bool IsOnline();

        /// <summary>
        /// Logic to send and receive buffer of bytes from scale
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<byte[]> SendAndReceiveAsync(byte[] request);
    }
}
