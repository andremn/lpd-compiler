using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LPD.Compiler.CodeGenerator.Instructions;

namespace LPD.Compiler.CodeGenerator
{
    public class CodeGenerator
    {
        private StringBuilder _output;

        public CodeGenerator()
        {
            _output = new StringBuilder();
        }

        public void GenerateLabel(int labelNumber)
        {
            GenerateInstruction("L" + labelNumber, NULL);
        }

        public void GenerateInstruction(string instructionName)
        {
            GenerateInstruction(instructionName, null);
        }

        public void GenerateInstruction(string instructionName, params string[] arguments)
        {
            _output.Append(instructionName);
            
            if (arguments == null)
            {
                return;
            }

            int index;

            for (index = 0; index < arguments.Length - 1; index++)
            {
                _output.Append(arguments[index]);
                _output.Append(",");
            }

            _output.Append(arguments[index]);
            _output.Append(Environment.NewLine);
        }

        public Task SaveToFileAsync(string path)
        {
            using (var file = File.Open(path, FileMode.Create))
            {
                using (var writer = new StreamWriter(file))
                {
                    return writer.WriteAsync(_output.ToString());
                }
            }
        }
    }
}
