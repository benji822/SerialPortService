using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SerialPortConnector
{
    internal sealed class PortConnect : IHostedService, IDisposable
    {
        private readonly ILogger<PortConnect> _logger;
        static SerialPort _serialPort;
        public PortConnect(ILogger<PortConnect> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {

            try
            {
                SerialPort _serialPort = new SerialPort("COM3");

                _serialPort.BaudRate = 9600;
                _serialPort.Parity = Parity.None;
                _serialPort.StopBits = StopBits.One;
                _serialPort.DataBits = 8;
                _serialPort.Handshake = Handshake.None;
                _serialPort.RtsEnable = true;

                _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

                _serialPort.Open();
                _logger.LogInformation($"Serial is open: {_serialPort.IsOpen}");

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex}");
            }
            return Task.CompletedTask;
        }

        private void DataReceivedHandler(
                        object sender,
                        SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            _logger.LogInformation("Data Received:");
            _logger.LogInformation(indata);
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("StopAsync Called");
            _serialPort?.Close();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _logger.LogInformation("Dispose Called");
            _serialPort?.Dispose();
        }
    }
}
