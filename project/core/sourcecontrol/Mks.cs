using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("mks")]
	public class Mks : ProcessSourceControl
	{
		public static readonly char DELIMITER = '|';

		//MKS arguments
		//configurable from ccnet.config
		private string executable = "";
		private string hostname = "";
		private string port = "";
		private string user = "";
		private string password = "";
		private string sandboxRoot = "";
		private string sandboxFile = "";

		//command templates
		private readonly string RESYNCH_COMMAND_TEMPLATE =
			"resync --quiet --overwriteChanged --restoreTimestamp --recurse --sandbox={0} --user={1} --password={2}";

/*
		private readonly string ADD_LABEL_COMMAND_TEMPLATE =
			"addprojectlabel --quiet --movelabel --user={1} --password={2} --label={3} -P={4}";
*/

		private readonly string MODIFICATIONS_COMMAND_TEMPLATE =
			"rlog --format=\"CCNet-{{workingrevdelta}}" + DELIMITER + " {{membername}}" + DELIMITER
				+ " {{revisioncount}}" + DELIMITER + " {{date}}" + DELIMITER + " {{author}}" + DELIMITER
				+ " {{description}}\" --recurse --sandbox={0} --user={1} --password={2}";

		[ReflectorProperty("executable")]
		public string Executable
		{
			get { return executable; }
			set { executable = value; }
		}

		[ReflectorProperty("hostname")]
		public string Hostname
		{
			get { return hostname; }
			set { hostname = value; }
		}

		[ReflectorProperty("port")]
		public string Port
		{
			get { return port; }
			set { port = value; }
		}

		[ReflectorProperty("user")]
		public string User
		{
			get { return user; }
			set { user = value; }
		}

		[ReflectorProperty("password")]
		public string Password
		{
			get { return password; }
			set { password = value; }
		}

		[ReflectorProperty("sandboxroot")]
		public string SandboxRoot
		{
			get { return sandboxRoot; }
			set { sandboxRoot = value; }
		}

		[ReflectorProperty("sandboxfile")]
		public string SandboxFile
		{
			get { return sandboxFile; }
			set { sandboxFile = value; }
		}

		public Mks() : this(new MksHistoryParser(), new ProcessExecutor())
		{
		}

		public Mks(IHistoryParser parser, ProcessExecutor executor) : base(parser, executor)
		{
		}


		public override void Initialize(IProject project)
		{
			base.Initialize(project);
		}

		public override Modification[] GetModifications(DateTime from, DateTime to)
		{
			ProcessInfo info = createProcess(MODIFICATIONS_COMMAND_TEMPLATE);
			Log.Info(string.Format("Getting Modifications on MKS: {0} {1}", info.FileName, info.Arguments));
			return base.GetModifications(info, from, to);
		}

		public override void LabelSourceControl(string label, IIntegrationResult result)
		{
/*
			string arguments = String.Format(ADD_LABEL_COMMAND_TEMPLATE,
			                                 SandboxRoot + "\\" + SandboxFile, User, Password, "_" + label, SandboxFile);
			ProcessInfo info = new ProcessInfo(Executable, arguments);
			Log.Info(string.Format("Labelling source on MKS: {0} {1}", info.FileName, info.Arguments));
			Execute(info);
*/

		}

		public override void GetSource(IIntegrationResult result)
		{
			ProcessInfo resynchProcess = createProcess(RESYNCH_COMMAND_TEMPLATE);
			Log.Info(string.Format("Getting source from MKS: {0} {1}", resynchProcess.FileName, resynchProcess.Arguments));
			Execute(resynchProcess);
			RemoveReadOnlyAttribute();
		}

		private void RemoveReadOnlyAttribute()
		{
			ProcessInfo attribProcess = new ProcessInfo("attrib", string.Format(" -R /s {0}", SandboxRoot + "\\*"));
			new ProcessExecutor().Execute(attribProcess);
		}

		private ProcessInfo createProcess(string processCommand)
		{
			string arguments = String.Format(processCommand, SandboxRoot + "\\" + SandboxFile, User, Password);
			return new ProcessInfo(Executable, arguments);
		}
	}
}