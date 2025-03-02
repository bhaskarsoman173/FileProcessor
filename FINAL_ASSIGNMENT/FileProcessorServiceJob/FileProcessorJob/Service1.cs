using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.IO;
using System.Xml.Linq;

namespace FileProcessorJob
{
    public partial class Service1 : ServiceBase
    {
        private string inputDirectoryPath = @"C:\FINAL_ASSIGNMENT\FileProcessorServiceJob\Paths\DropMeHere";
        private string outputFilePath = @"C:\FINAL_ASSIGNMENT\FileProcessorServiceJob\Paths\OutputXml\OutputFile.txt";
        private string processedDirectoryPath= @"C:\FINAL_ASSIGNMENT\FileProcessorServiceJob\Paths\DropMeHere\processed";
        
        private Timer timer = new Timer();
        public Service1()
        {
            InitializeComponent();
            //processedDirectoryPath = Path.Combine(inputDirectoryPath, "processed"); // Define processed folder path
        }

        protected override void OnStart(string[] args)
        {
            WriteToFile("Service is started at " + DateTime.Now);
            timer.Elapsed += new ElapsedEventHandler(onElapsedTime);
            timer.Interval = 5000;//milliseconds
            timer.Enabled=true;

        }

        protected override void OnStop()
        {
            WriteToFile("Service is stopped at " + DateTime.Now);
        }

        private void onElapsedTime(object sender, ElapsedEventArgs e)
        {
            WriteToFile("Checking for .txt files at " + DateTime.Now);
            ProcessTextFiles();
        }




        //private void ProcessTextFiles()
        //{
        //    try
        //    {
        //        // Ensure the "processed" folder exists
        //        if (!Directory.Exists(processedDirectoryPath))
        //        {
        //            Directory.CreateDirectory(processedDirectoryPath);
        //        }

        //        var files = Directory.GetFiles(inputDirectoryPath, "*.txt");
        //        foreach (var file in files)
        //        {
        //            WriteToFile($"Found file: {file}");

        //            // Read content from the original file
        //            string fileContent = File.ReadAllText(file);

        //            // Write the content to the output file
        //            File.WriteAllText(outputFilePath, fileContent);

        //            // Move the original file to the "processed" folder
        //            string processedFilePath = Path.Combine(processedDirectoryPath, Path.GetFileName(file));
        //            File.Move(file, processedFilePath);

        //            WriteToFile($"Processed and moved file: {file} to {processedFilePath}");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        EventLog.WriteEntry("FileProcessorJob", ex.ToString(), EventLogEntryType.Error);
        //    }
        //}



        private void ProcessTextFiles()
        {
            try
            {
                // Ensure the "processed" folder exists
                if (!Directory.Exists(processedDirectoryPath))
                {
                    Directory.CreateDirectory(processedDirectoryPath);
                }

                var files = Directory.GetFiles(inputDirectoryPath, "*.txt");
                foreach (var file in files)
                {
                    WriteToFile($"Found file: {file}");

                    // Read all lines from the original file
                    var lines = File.ReadAllLines(file);
                    if (lines.Length < 2) // Ensure there are lines to process
                        continue;

                    // Create the XML structure
                    var xmlDoc = new XDocument(new XElement("Orders"));
                    string currentOrderNumber = null;
                    XElement currentOrder = null;

                    foreach (var line in lines.Skip(1)) // Skip header
                    {
                        var columns = line.Split('|');
                        if (columns.Length < 12) // Check for the right number of columns
                        {
                            WriteToFile($"Invalid line format: {line}");
                            continue; // Skip this line
                        }

                        // Extract fields
                        string orderNumber = columns[0];
                        string orderDate = columns[8];
                        string customerName = columns[9];
                        string customerNumber = columns[10];
                        string orderLineNumber = columns[1];
                        string productNumber = columns[2];
                        string quantity = columns[3];
                        string name = columns[4];
                        string description = columns[5];
                        string price = columns[6];
                        string productGroup = columns[7];

                        // Check if we're still on the same order
                        if (currentOrderNumber != orderNumber)
                        {
                            // Create a new order element
                            currentOrder = new XElement("Order",
                                new XElement("OrderNumber", orderNumber),
                                new XElement("OrderDate", orderDate),
                                new XElement("CustomerName", customerName),
                                new XElement("CustomerNumber", customerNumber));

                            xmlDoc.Root.Add(currentOrder);
                            currentOrderNumber = orderNumber;
                        }

                        // Create order line element
                        var orderLine = new XElement("Orderline",
                            new XElement("OrderLineNumber", orderLineNumber),
                            new XElement("ProductNumber", productNumber),
                            new XElement("Quantity", quantity),
                            new XElement("Name", name),
                            new XElement("Description", description),
                            new XElement("Price", price),
                            new XElement("ProductGroup", productGroup));

                        currentOrder.Add(orderLine);
                    }

                    // Save the XML document to the output file
                    xmlDoc.Save(outputFilePath);

                    // Move the original file to the "processed" folder
                    string processedFilePath = Path.Combine(processedDirectoryPath, Path.GetFileName(file));
                    File.Move(file, processedFilePath);

                    WriteToFile($"Processed and moved file: {file} to {processedFilePath}");
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("FileProcessorJob", ex.ToString(), EventLogEntryType.Error);
            }
        }


        public void WriteToFile(string message)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string filePath = Path.Combine(path, $"ServiceLog_{DateTime.Now:yyyy-MM-dd}.txt");

            try
            {
                lock (path) // Simple lock for thread safety
                {
                    using (StreamWriter sw = File.Exists(filePath) ? File.AppendText(filePath) : File.CreateText(filePath))
                    {
                        sw.WriteLine($"{DateTime.Now}: {message}");
                    }
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("FileProcessorJob", ex.ToString(), EventLogEntryType.Error);
            }
        }






    }
}
