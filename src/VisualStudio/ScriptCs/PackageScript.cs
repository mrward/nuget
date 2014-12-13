// 
// PackageScript.cs
// 
// Author:
//   Matt Ward <ward.matt@gmail.com>
// 
// Copyright (C) 2011-2014 Matthew Ward
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using EnvDTE;
using NuGet;

namespace NuGet.VisualStudio
{
	public class PackageScript
	{
        public PackageScript(string fileName)
        {
            FileName = fileName;
        }

        public string FileName { get; set; }
		public IPackage Package { get; set; }
		public Project Project { get; set; }
        public string ToolsPath { get; set; }
        public string RootPath { get; set; }

        ScriptCsSession session;

        public void Run(ScriptCsSession session)
		{
			this.session = session;
			Run();
		}
		
		void Run()
		{
			AddSessionVariables();
			RunScript();
			RemoveSessionVariables();
		}
		
		void AddSessionVariables()
		{
			session.AddVariable("__rootPath", RootPath);
			session.AddVariable("__toolsPath", ToolsPath);
			session.AddVariable("__package", Package);
			session.AddVariable("__project", Project);
		}
		
		void RunScript()
		{
			session.InvokeScript(FileName);
		}
		
		void RemoveSessionVariables()
		{
			session.RemoveVariable("__rootPath");
			session.RemoveVariable("__toolsPath");
			session.RemoveVariable("__package");
			session.RemoveVariable("__project");
		}
	}
}
