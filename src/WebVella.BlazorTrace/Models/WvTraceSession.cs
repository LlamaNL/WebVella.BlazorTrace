﻿using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WebVella.BlazorTrace.Utility;

namespace WebVella.BlazorTrace.Models;
public class WvTraceSessionModule
{
	public Dictionary<string, WvTraceSessionComponent> ComponentDict { get; set; } = new();
}

public class WvTraceSessionComponent
{
	public string? Name { get; set; }
	public List<WvTraceSessionComponentTaggedInstance> TaggedInstances { get; set; } = new();
}

public class WvTraceSessionComponentTaggedInstance
{
	public string? Tag { get; set; }
	public WvTraceSessionMethod OnInitialized { get; set; } = new();
	public WvTraceSessionMethod OnParameterSet { get; set; } = new();
	public WvTraceSessionMethod OnAfterRender { get; set; } = new();
	public WvTraceSessionMethod ShouldRender { get; set; } = new();
	public WvTraceSessionMethod Dispose { get; set; } = new();
	public List<WvTraceSessionMethod> OtherMethods { get; set; } = new();

	public List<WvTraceSessionMethod> MethodsTotal()
	{
		var methods = new List<WvTraceSessionMethod>();
		methods.Add(OnInitialized);
		methods.Add(OnParameterSet);
		methods.Add(OnAfterRender);
		methods.Add(ShouldRender);
		methods.Add(Dispose);
		methods.AddRange(OtherMethods);
		var calledMethods = new List<WvTraceSessionMethod>();
		foreach (var method in methods)
		{
			if (method.MaxCallsCount > 0)
				calledMethods.Add(method);
		}
		return calledMethods;
	}
}

public class WvTraceSessionMethod
{
	public string? Name { get; set; } = null;
	public List<WvTraceSessionMethodTrace> TraceList { get; set; } = new();
	//Generated props
	[JsonIgnore]
	public long? MinDurationMs { get => this.GetMinDuration(); }
	[JsonIgnore]
	public long? MaxDurationMs { get => this.GetMaxDuration(); }
	[JsonIgnore]
	public long? LastDurationMS { get => this.GetLastDuration(); }
	[JsonIgnore]
	public long? OnEnterMinMemoryBytes { get => this.GetMinMemory(isOnEnter: true); }
	[JsonIgnore]
	public long? OnEnterMaxMemoryBytes { get => this.GetMaxMemory(isOnEnter: true); }
	[JsonIgnore]
	public long? OnExitMinMemoryBytes { get => this.GetMinMemory(isOnEnter: false); }
	[JsonIgnore]
	public long? OnExitMaxMemoryBytes { get => this.GetMaxMemory(isOnEnter: false); }
	[JsonIgnore]
	public long? LastMemoryBytes { get => this.GetLastMemory().Item1; }
	[JsonIgnore]
	public List<WvTraceMemoryInfo>? LastMemoryInfo { get => this.GetLastMemory().Item2; }
	[JsonIgnore]
	public long? MinMemoryDeltaBytes { get => this.GetMinMemoryDelta(); }
	[JsonIgnore]
	public long? MaxMemoryDeltaBytes { get => this.GetMaxMemoryDelta(); }
	[JsonIgnore]
	public long OnEnterCallsCount { get => this.GetOnEnterCallCount(); }
	[JsonIgnore]
	public long OnExitCallsCount { get => this.GetOnExitCallsCount(); }
	[JsonIgnore]
	public long MaxCallsCount { get => this.GetMaxCallsCount(); }
	[JsonIgnore]
	public long CompletedCallsCount { get => this.CompletedCallsCount(); }
	[JsonIgnore]
	public WvTraceSessionMethodTrace? LastExitedTrace
	{
		get => TraceList.Where(x => x.ExitedOn is not null).OrderByDescending(x => x.ExitedOn).FirstOrDefault();
	}
	[JsonIgnore]
	public List<WvTraceSessionLimitHit> LimitHits { get => this.CalculateLimitsInfo(); }
	public string GenerateHash(string moduleName, string componentFullname, string? tag)
	{
		return WvModalUtility.GenerateMethodHash(moduleName, componentFullname, tag, Name);
	}
}


public class WvTraceSessionMethodTrace
{
	public Guid TraceId { get; set; } = default!;
	public DateTimeOffset? EnteredOn { get; set; } = null;
	public DateTimeOffset? ExitedOn { get; set; } = null;
	public long? OnEnterMemoryBytes { get; set; } = null;
	public long? OnExitMemoryBytes { get; set; } = null;
	public string? OnEnterCustomData { get; set; } = null;
	public string? OnExitCustomData { get; set; } = null;
	public WvTraceMethodOptions OnEnterOptions { get; set; } = new();
	public WvTraceMethodOptions OnExitOptions { get; set; } = new();

	//Non Serializable - to much info to be stored in the store JSON
	[JsonIgnore]
	public List<WvTraceMemoryInfo>? OnEnterMemoryInfo { get; set; } = null;

	[JsonIgnore]
	public List<WvTraceMemoryInfo>? OnExitMemoryInfo { get; set; } = null;

	//Generated props
	[JsonIgnore]
	public long? DurationMs { get => this.GetDurationMS(); }
	[JsonIgnore]
	public string OnEnterLimitsHTML { get => this.CalculateLimitsHTML(true); }
	[JsonIgnore]
	public string OnExitLimitsHTML { get => this.CalculateLimitsHTML(false); }
}

public class WvTraceSessionSignal
{
	public List<WvTraceSessionSignalTrace> TraceList { get; set; } = new();

	[JsonIgnore]
	public List<WvTraceSessionLimitHit> LimitHits { get => this.CalculateLimitsInfo(); }
}

public class WvTraceSessionSignalTrace
{
	public DateTimeOffset SendOn { get; set; } = default!;
	public string? ModuleName { get; set; } = null;
	public string? ComponentName { get; set; } = null;
	public string? ComponentFullName { get; set; } = null;
	public string? InstanceTag { get; set; } = null;
	public string? MethodName { get; set; } = null;
	public string? CustomData { get; set; } = null;
	public WvTraceSignalOptions Options { get; set; } = default!;
}


public class WvTraceSessionLimitHit
{
	public bool IsOnEnter { get; set; } = true;
	public WvTraceSessionLimitType Type { get; set; }
	public long Limit { get; set; } = 0;
	public long Actual { get; set; } = 0;
}

public enum WvTraceSessionLimitType
{
	[Description("calls count")]
	CallCount = 0,
	[Description("duration")]
	Duration = 1,
	[Description("total memory")]
	MemoryTotal = 2,
	[Description("memory delta")]
	MemoryDelta = 3
}