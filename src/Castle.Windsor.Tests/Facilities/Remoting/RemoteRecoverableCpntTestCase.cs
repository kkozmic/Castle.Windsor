// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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


#if (!SILVERLIGHT)

namespace CastleTests.Facilities.Remoting
{
	using System;
	using System.Runtime.Remoting;

	using CastleTests.Components;

	using NUnit.Framework;

	[TestFixture]
	[Serializable]
	public class RemoteRecoverableCpntTestCase : AbstractRemoteTestCase
	{
		protected override String GetServerConfigFile()
		{
			return "server_kernelcomponent_recover.xml";
		}

		private void StartServer()
		{
			serverDomain = AppDomainFactory.Create("server");
			serverContainer = CreateRemoteContainer(serverDomain, GetServerConfigFile());
		}

		private void ShutdownSever()
		{
			serverContainer.Dispose();
			AppDomain.Unload(serverDomain);
		}

		public void ClientContainerInvokingRemoteComponent()
		{
			var clientContainer = GetRemoteContainer(clientDomain, "client_kernelcomponent_recover.xml");

			var service = clientContainer.Resolve<ICalcService>();

			Assert.IsTrue(RemotingServices.IsTransparentProxy(service));
			Assert.IsTrue(RemotingServices.IsObjectOutOfAppDomain(service));

			Assert.AreEqual(10, service.Sum(7, 3));
		}

		[Test]
		public void ServerRestarted()
		{
			clientDomain.DoCallBack(ClientContainerInvokingRemoteComponent);

			ShutdownSever();

			StartServer();

			clientDomain.DoCallBack(ClientContainerInvokingRemoteComponent);
		}
	}
}

#endif