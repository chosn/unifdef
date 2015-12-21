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
        String condition;
        public IfDefItem (int line)
        {
            if_statement_line = line;
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
        public List<IfDefItem> ifDefBlocks = new List<IfDefItem>();

        public String GetItemCondition(IfDefItem item)
        {
            return fileContent.ElementAt(item.if_statement_line);
        }

        public ProgramFile(String filepath)
        {
            file = new System.IO.StreamReader(filepath);
            string line;
            int counter = 0;

            while ((line = file.ReadLine()) != null)
            {
                fileContent.Add(line);
                //System.Console.WriteLine(line);
                counter++;
            }
            file.Close();
            parseForIfDefItems();
            parseForDefines();
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

        public void parseForIfDefItems()
        {
            IfDefItem newElement;
            int line = 0;
            int index = 0;

            while (line < fileContent.Count)
            {
                if (fileContent.ElementAt(line).Contains("#if"))
                {
                    newElement = new IfDefItem(line);
                    ifDefBlocks.Add(newElement);
                    index++;
                }
                else if (fileContent.ElementAt(line).Contains("#endif"))
                {
                    index--;
                    ifDefBlocks.ElementAt(index).endif_statement_line = line;
                }
                line++;
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            ProgramFile app_config = new ProgramFile(@"C:\Temp\ifdeftest.txt");
            
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
