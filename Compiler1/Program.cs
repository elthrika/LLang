using System.IO;
using System;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace Compiler1
{
    class Program
    {
        static void Main(string[] args)
        {

            string filename = args[0];
            string outfile = filename + ".s";

            AntlrInputStream ais = new AntlrInputStream(File.Open(filename, FileMode.Open));
            ITokenSource lexer = new llangLexer(ais);
            ITokenStream tokens = new CommonTokenStream(lexer);
            llangParser parser = new llangParser(tokens);
            parser.BuildParseTree = true;
            IParseTree tree = parser.prog();

            ASTBuilder b = new ASTBuilder();
            ASTNode root = b.Visit(tree);

            TypeMaker tm = new TypeMaker();
            tm.Visit(root);

            SemanticChecker sc = new SemanticChecker();
            int errors = 0;

            if ((errors = sc.Check(root)) > 0)
            {
                Console.WriteLine("{0} Error(s) - Aborting", errors);
                return;
            }

            (new AstPrinter()).Visit(root);

            Interpreter interp = new Interpreter(root);
            try
            {
                interp.Run();
            }
            catch (Interpreter.ExecutionTerminatedException e)
            {
                Console.WriteLine($"Execution Terminated by exit({e.ExitCode}) call");
            }

            //(new MIPSCodeGenerator(outfile)).Visit(root);
            //Console.ReadKey();
        }
    }
}