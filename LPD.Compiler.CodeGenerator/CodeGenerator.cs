using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using static LPD.Compiler.CodeGeneration.Instructions;

namespace LPD.Compiler.CodeGeneration
{
    public class CodeGenerator
    {
        private StringBuilder _output;

        public CodeGenerator()
        {
            _output = new StringBuilder();
        }

        public void GenerateLabel(uint labelNumber) => GenerateInstruction("L" + labelNumber, NULL);

        public void GenerateInstruction(string instructionName) => GenerateInstruction(instructionName, null);

        public void GenerateInstruction(string instructionName, params object[] arguments)
        {
            _output.Append(instructionName);

            if (arguments == null)
            {
                _output.Append(Environment.NewLine);
                return;
            }

            _output.Append(" ");

            int index;

            for (index = 0; index < arguments.Length - 1; index++)
            {                
                _output.Append(arguments[index]);
                _output.Append(",");
            }

            _output.Append(arguments[index]);
            _output.Append(Environment.NewLine);
        }

        public string GetStringLabelFor(uint labelNumber) => "L" + labelNumber;

        public Task SaveToFileAsync(string path)
        {
            using (var file = File.Open(path, FileMode.Create, FileAccess.Write))
            {
                using (var writer = new StreamWriter(file))
                {
                    return writer.WriteAsync(_output.ToString());
                }
            }
        }
    }
}
