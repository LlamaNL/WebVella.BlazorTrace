﻿using WebVella.BlazorTrace.Models;

namespace WebVella.BlazorTrace.Tests;

public partial class WvBlazorTraceServiceTests : BaseTest
{

	[Fact]
	public async Task OnEnterTestBaseCalls()
	{
		using (await _locker.LockAsync())
		{
			//given
			var options = new WvTraceMethodOptions { };
			var moduleName = "WebVella.BlazorTrace.Tests";
			var componentName = "TestComponent";
			var componentFullName = moduleName + "." + componentName;
			var methodsList = new List<string>{
				"OnInitialized","OnInitializedAsync",
				"OnParametersSet","OnParametersSetAsync",
				"OnAfterRender","OnAfterRenderAsync",
				"ShouldRender",
				"Dispose","DisposeAsync",
				"OnEnterTestOther","OnEnterTestOtherAsync"
			};

			foreach (var methodName in methodsList)
			{
				var traceId = Guid.NewGuid();
				var instanceTag = Guid.NewGuid().ToString();
				var customData = Guid.NewGuid().ToString();
				//when
				Action action = () => WvBlazorTraceServiceMock.Object.OnEnter(
					caller: Component,
					traceId: traceId,
					options: options,
					instanceTag: instanceTag,
					customData: customData,
					methodName: methodName
				);
				var ex = Record.Exception(action);
				Assert.Null(ex);
				//than
				var queue = WvBlazorTraceServiceMock.Object.GetQueue();
				Assert.NotEmpty(queue);
				WvBlazorTraceServiceMock.Object.ProcessQueue();
				var (method, trace) = CheckTraceExists(
					moduleDict: WvBlazorTraceServiceMock.Object.GetModuleDict(),
					moduleName: moduleName,
					componentFullName: componentFullName,
					componentName: componentName,
					instanceTag: instanceTag,
					methodName: methodName,
					traceId: traceId
				);
				Assert.NotNull(trace.EnteredOn);
				Assert.NotNull(trace.OnEnterOptions);
				Assert.Null(trace.ExitedOn);
			}
		}
	}

	[Fact]
	public async Task OnExitTestBaseCalls()
	{
		using (await _locker.LockAsync())
		{
			//given
			var options = new WvTraceMethodOptions { };
			var moduleName = "WebVella.BlazorTrace.Tests";
			var componentName = "TestComponent";
			var componentFullName = moduleName + "." + componentName;
			var methodsList = new List<string>{
				"OnInitialized","OnInitializedAsync",
				"OnParametersSet","OnParametersSetAsync",
				"OnAfterRender","OnAfterRenderAsync",
				"ShouldRender",
				"Dispose","DisposeAsync",
				"OnEnterTestOther","OnEnterTestOtherAsync"
			};
			foreach (var methodName in methodsList)
			{
				var traceId = Guid.NewGuid();
				var instanceTag = Guid.NewGuid().ToString();
				var customData = Guid.NewGuid().ToString();
				//when
				Action action = () => WvBlazorTraceServiceMock.Object.OnExit(
					caller: Component,
					traceId: traceId,
					options: options,
					instanceTag: instanceTag,
					customData: customData,
					methodName: methodName
				);
				var ex = Record.Exception(action);
				Assert.Null(ex);
				//than
				var queue = WvBlazorTraceServiceMock.Object.GetQueue();
				Assert.NotEmpty(queue);
				WvBlazorTraceServiceMock.Object.ProcessQueue();
				var (method, trace) = CheckTraceExists(
					moduleDict: WvBlazorTraceServiceMock.Object.GetModuleDict(),
					moduleName: moduleName,
					componentFullName: componentFullName,
					componentName: componentName,
					instanceTag: instanceTag,
					methodName: methodName,
					traceId: traceId
				);
				Assert.Null(trace.EnteredOn);
				Assert.NotNull(trace.ExitedOn);
				Assert.NotNull(trace.OnExitOptions);
			}
		}
	}

	[Fact]
	public async Task SignalTestBaseCalls()
	{
		using (await _locker.LockAsync())
		{
			//given
			var options = new WvTraceSignalOptions { };
			var moduleName = "WebVella.BlazorTrace.Tests";
			var signalName = "test-signal";
			var componentName = "TestComponent";
			var componentFullName = moduleName + "." + componentName;
			var methodName = "CustomMethod";

			var traceId = Guid.NewGuid();
			var instanceTag = Guid.NewGuid().ToString();
			var customData = Guid.NewGuid().ToString();
			//when
			Action action = () => WvBlazorTraceServiceMock.Object.OnSignal(
				caller: Component,
				signalName: signalName,
				instanceTag: instanceTag,
				customData: customData,
				options: options,
				methodName: methodName
			);
			var ex = Record.Exception(action);
			Assert.Null(ex);
			//than
			var queue = WvBlazorTraceServiceMock.Object.GetQueue();
			Assert.NotEmpty(queue);
			WvBlazorTraceServiceMock.Object.ProcessQueue();
			var (method, trace) = CheckSignalTraceExists(
				signalDict: WvBlazorTraceServiceMock.Object.GetSignalDict(),
				moduleName: moduleName,
				signalName: signalName,
				componentFullName: componentFullName,
				instanceTag: instanceTag,
				methodName: methodName
			);
			Assert.NotNull(trace.SendOn);
			Assert.NotNull(trace.Options);
		}
	}
}
