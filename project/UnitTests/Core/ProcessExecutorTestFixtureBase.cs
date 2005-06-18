using System;
using System.ComponentModel;
using NMock;
using NMock.Constraints;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
	public class ProcessExecutorTestFixtureBase : CustomAssertion
	{
		protected const int SuccessfulExitCode = 0;
		protected const int FailedExitCode = -1;
		protected string DefaultWorkingDirectory = @"c:\source\";
		protected int DefaultTimeout = ProcessSourceControl.DEFAULT_TIMEOUT;

		protected IMock mockProcessExecutor;
		protected string defaultExecutable;

		protected void CreateProcessExecutorMock(string executable)
		{
			mockProcessExecutor = new DynamicMock(typeof(ProcessExecutor)); 
			mockProcessExecutor.Strict = true;
			defaultExecutable = executable;
		}

		protected void Verify()
		{
			mockProcessExecutor.Verify();	
		}

		protected void ExpectThatExecuteWillNotBeCalled()
		{
			mockProcessExecutor.ExpectNoCall("Execute", typeof(ProcessInfo));
		}

		protected void ExpectToExecuteArguments(string args)
		{
			ExpectToExecute(NewProcessInfo(args));
		}

		protected void ExpectToExecute(ProcessInfo processInfo)
		{
			mockProcessExecutor.ExpectAndReturn("Execute", SuccessfulProcessResult(), processInfo);
		}

		protected void ExpectToExecuteAndReturn(ProcessResult result)
		{
			mockProcessExecutor.ExpectAndReturn("Execute", result, new IsAnything());
		}

		protected void ExpectToExecuteAndThrow()
		{
			mockProcessExecutor.ExpectAndThrow("Execute", new Win32Exception(), new IsAnything());
		}

		protected virtual IIntegrationResult IntegrationResult()
		{
			return IntegrationResult(DateTime.Now);
		}

		protected IIntegrationResult IntegrationResult(DateTime start)
		{
			IntegrationResult successful = IntegrationResultMother.CreateSuccessful(start);
			successful.WorkingDirectory = DefaultWorkingDirectory;
			return successful;
		}

		protected ProcessResult SuccessfulProcessResult()
		{
			return ProcessResultFixture.CreateSuccessfulResult("output");
		}

		protected ProcessResult FailedProcessResult()
		{
			return new ProcessResult("output", null, FailedExitCode, false);
		}

		protected ProcessResult TimedOutProcessResult()
		{
			return ProcessResultFixture.CreateTimedOutResult();
		}

		protected ProcessInfo NewProcessInfo(string args)
		{
			ProcessInfo info = new ProcessInfo(defaultExecutable, args, DefaultWorkingDirectory);
			info.TimeOut = DefaultTimeout;
			return info;
		}		
	}
}