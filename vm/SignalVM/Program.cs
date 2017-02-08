using System;
using System.IO;

namespace SignalVM
{
	class MainClass
	{
		public static void Main(string[] args)
		{
            new VirtualMachine(File.ReadAllBytes(args[0])).Execute();
		}
	}
}
