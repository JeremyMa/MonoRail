﻿// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.MonoRail.Framework.Tests.Actions
{
	using System.Collections.Generic;
	using System.Reflection;
	using Castle.MonoRail.Framework.Descriptors;
	using Castle.MonoRail.Framework.Test;
	using NUnit.Framework;

	[TestFixture]
	public class ActionMethodExecutorCompatibleTestCase
	{
		[Test]
		public void CompatibleExecutorDelegatesInvocationToControllerUsingDelegate()
		{
			var controller = new BaseController();
			var actionMeta = new ActionMetaDescriptor();

			ActionMethodExecutorCompatible.InvokeOnController delegateToController = controller.InvokeMethodStub;

			var executor = 
				new ActionMethodExecutorCompatible(GetActionMethod(controller), actionMeta, delegateToController);

			var req = new StubRequest();
			var res = new StubResponse();
			var services = new StubMonoRailServices();
			IEngineContext engineContext = new StubEngineContext(req, res, services, new UrlInfo("area", "controller", "action"));
			var retVal = executor.Execute(engineContext, controller, new ControllerContext());

			Assert.IsTrue(controller.WasExecuted);
			Assert.AreEqual(2, retVal);
		}

		private MethodInfo GetActionMethod(object controller)
		{
			return controller.GetType().GetMethod("Action1");
		}

		public class BaseController : Controller
		{
			private bool wasExecuted;

			public object Action1()
			{
				return 1;
			}

			public bool WasExecuted
			{
				get { return wasExecuted; }
			}

			public object InvokeMethodStub(MethodInfo method, IRequest request, IDictionary<string,object> methodArgs)
			{
				wasExecuted = true;
				return 2;
			}
		}
	}
}
