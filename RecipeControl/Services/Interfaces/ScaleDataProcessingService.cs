using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.Services.Interfaces
{
    /// <summary>
    /// Data processing service for scales using ASCII encoding
    /// </summary>
    public class ScaleDataProcessingService : IScaleDataProcessingService
    {
        /// <summary>
        /// Build weight request message
        /// </summary>
        /// <returns></returns>
        public byte[] BuildWeightRequest()
        {
            var message = "RW\r\n";
            var bytesToSend = Encoding.ASCII.GetBytes(message);
            Debug.WriteLine(bytesToSend);
            return bytesToSend;
        }

        /// <summary>
        /// Parse weight response message
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        public decimal ParseWeightResponse(byte[] response)
        {
            // Decode string from byte[] response
            string frame = Encoding.ASCII.GetString(response).Trim();

            // Validate frame format and retrive value
            if (frame.StartsWith("WR:"))
            {
                var value = frame.Substring(3);
                if (decimal.TryParse(value, out decimal w))
                {
                    return w;
                }
            }

            // Throw FormatException on invalide response format
            throw new FormatException($"Invalid frame: {frame}");
        }
    }
}
