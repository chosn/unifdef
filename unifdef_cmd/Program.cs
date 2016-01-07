using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace unifdef_cmd
{
    class IfDefItem
    {
        public int if_statement_line;
        List<int?> else_statement_line = new List<int?>();
    
        public int endif_statement_line;
        bool active;
        public String condition;
        public IfDefItem (int line)
        {
            if_statement_line = line;
        }
        public IfDefItem()
        {
        }
    }
    
    class DefineType
    {
        public String name;
        public int location;
        public bool active;
        public DefineType(int line, String text)
        {
            int position;

            location = line;
            name = text.Trim();
            position = name.IndexOf("//#define");
            if (position == -1)
            {
                position = name.IndexOf("#define");
                if (position == -1)
                {
                    //error
                }
                else
                {
                    position += "#define".Count();
                    active = true;
                }
            }
            else
            {
                position += "//define".Count();
                active = false;
            }
        }
    }

    class ProgramFile
    {
        System.IO.StreamReader file;
        public List<String> fileContent = new List<String>();
        public List<DefineType> DefinesList = new List<DefineType>();
        public List<IfDefItem> ifDefBlocks;

        public String GetItemCondition(IfDefItem item)
        {
            return fileContent.ElementAt(item.if_statement_line);
        }

        public ProgramFile(String filepath)
        {
            file = new System.IO.StreamReader(filepath);
            string line;
            int counter = 0;
            fileContent.Add(filepath);
            while ((line = file.ReadLine()) != null)
            {
                fileContent.Add(line);
                //System.Console.WriteLine(line);
                counter++;
            }
            file.Close();
            //parseForIfDefItems();
            //parseForDefines();

            ifDefBlocks = sucheIfElseEnd();
        }

        public void parseForDefines()
        {
            DefineType newDefine;
            int line = 0;

            while (line < fileContent.Count)
            {
                if (fileContent.ElementAt(line).Contains("#define"))
                {
                    newDefine = new DefineType(line, fileContent.ElementAt(line));
                    DefinesList.Add(newDefine);
                }
                line++;
            }
        }

        public List<IfDefItem> sucheIfElseEnd(int zeile = 0)
        {
            int? ifZeile = null;
            int? elseZeile = 0;
            int? endifZeile = 0;
            int index;
            List<IfDefItem> retVal = new List<IfDefItem>();
            IfDefItem newElement = new IfDefItem();
            String text;

            while (zeile < fileContent.Count)
            {
                text = fileContent.ElementAt(zeile);
                if (fileContent.ElementAt(zeile).Contains("#ifdef"))
                {
                    if (ifZeile == null)
                    {
                        newElement.if_statement_line = zeile;
                        retVal.Add(newElement);
                        index = fileContent.ElementAt(zeile).IndexOf("#ifdef") + 7;
                        if (fileContent.ElementAt(zeile).Length <= index)
                        {
                            break; // error: no condition found
                        }
                        newElement.condition = fileContent.ElementAt(zeile).Substring(index);
                        ifZeile = zeile;
                        elseZeile = null;
                        endifZeile = null;
                    }
                    else
                    {   // new additional ifdef found
                        retVal.AddRange(sucheIfElseEnd(zeile));
                        zeile = retVal.Last().endif_statement_line;
                    }
                }
                else if (fileContent.ElementAt(zeile).Contains("#else"))
                {
                    if (elseZeile == null)
                    {
                        elseZeile = zeile;
                    }
                    else
                    {   // error
                        elseZeile = null;
                        break;
                    }
                }
                else if (fileContent.ElementAt(zeile).Contains("#endif"))
                {
                    if (endifZeile == null)
                    {
                        endifZeile = zeile;
                        newElement.endif_statement_line = zeile;
                    }
                    else
                    {   // error
                        endifZeile = null;
                    }
                    break;
                }
                zeile++;
            }
            zeile = 0;
            return retVal;
        }
    }

    class Program
    {
        
        static void udpSend()
        {
            System.Net.Sockets.UdpClient milight = new System.Net.Sockets.UdpClient("192.168.1.146", 8899);
            milight.Send(new byte[] { 0x41, 0x00, 0x55 }, 3);
            milight.Close();

            System.IO.Ports.SerialPort serialPort1 = new System.IO.Ports.SerialPort();

            serialPort1.PortName = "COM6";
            serialPort1.BaudRate = 9600;

            serialPort1.Open();
            if (serialPort1.IsOpen)
            {
                serialPort1.Write(new byte[] { 0x42, 0x00, 0x55 }, 0, 3);
            }
            serialPort1.Close();
        }
        static void Main(string[] args)
        {
            ProgramFile app_config = new ProgramFile(@"C:\Temp\ifdeftest.txt");
            //udpSend();
            //string line;

            // Read the file and display it line by line.
            //System.IO.StreamReader file = new System.IO.StreamReader(@"C:\Temp\app_config.h");
            /*
            while ((line = file.ReadLine()) != null)
            {
                app_config.ifdef_lines.Add(line);
                //System.Console.WriteLine(line);
                counter++;
            }

            file.Close();
            
            System.Console.WriteLine("There were {0} lines.", counter);
            */
            //app_config.GetIfdefLines();
            // Suspend the screen.
            System.Console.ReadLine();
        }
    }
}
