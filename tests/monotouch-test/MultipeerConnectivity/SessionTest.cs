//
// MCSession Unit Tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc.
//

#if !__TVOS__ && !__WATCHOS__

using System;
#if XAMCORE_2_0
using Foundation;
using UIKit;
using MultipeerConnectivity;
using ObjCRuntime;
using Security;
#else
using MonoTouch.Foundation;
using MonoTouch.MultipeerConnectivity;
using MonoTouch.ObjCRuntime;
using MonoTouch.Security;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

using MonoTouchFixtures.Security;

#if XAMCORE_2_0
using RectangleF=CoreGraphics.CGRect;
using SizeF=CoreGraphics.CGSize;
using PointF=CoreGraphics.CGPoint;
#else
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
#endif
namespace MonoTouchFixtures.MultipeerConnectivity {

	[TestFixture]
	// we want the test to be availble if we use the linker
	[Preserve (AllMembers = true)]
	public class SessionTest {

		[Test]
		public void CtorPeer ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (7, 0))
				Assert.Ignore ("requires iOS7+");

			using (var peer = new MCPeerID ("me"))
			using (var s = new MCSession (peer)) {
				Assert.AreSame (s.MyPeerID, peer, "MyPeerID");
				Assert.Null (s.SecurityIdentity, "SecurityIdentity");
				var pref = UIDevice.CurrentDevice.CheckSystemVersion (9,0) ? MCEncryptionPreference.Required : MCEncryptionPreference.Optional;
				Assert.That (s.EncryptionPreference, Is.EqualTo (pref), "EncryptionPreference");
				Assert.That (s.ConnectedPeers, Is.Empty, "ConnectedPeers");
			}
		}

		[Test]
		public void Ctor_OptionalIdentity ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (7, 0))
				Assert.Ignore ("requires iOS7+");

			using (var peer = new MCPeerID ("me"))
			using (var s = new MCSession (peer, null, MCEncryptionPreference.None)) {
				Assert.AreSame (s.MyPeerID, peer, "MyPeerID");
				Assert.Null (s.SecurityIdentity, "SecurityIdentity");
				Assert.That (s.EncryptionPreference, Is.EqualTo (MCEncryptionPreference.None), "EncryptionPreference");
				Assert.That (s.ConnectedPeers, Is.Empty, "ConnectedPeers");
			}
		}

		[Test]
		public void Ctor_Identity ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (7, 0))
				Assert.Ignore ("requires iOS7+");

			using (var id = IdentityTest.GetIdentity ())
			using (var peer = new MCPeerID ("me"))
			using (var s = new MCSession (peer, id, MCEncryptionPreference.Required)) {
				Assert.AreSame (s.MyPeerID, peer, "MyPeerID");
				Assert.That (s.SecurityIdentity.Count, Is.EqualTo ((nuint) 1), "SecurityIdentity");
				Assert.That (s.SecurityIdentity.GetItem<SecIdentity> (0).Handle, Is.EqualTo (id.Handle), "SecurityIdentity");
				Assert.That (s.EncryptionPreference, Is.EqualTo (MCEncryptionPreference.Required), "EncryptionPreference");
				Assert.That (s.ConnectedPeers, Is.Empty, "ConnectedPeers");
			}
		}

		[Test]
		public void Ctor_Identity_Certificates ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (7, 0))
				Assert.Ignore ("requires iOS7+");

			using (var id = IdentityTest.GetIdentity ())
			using (var trust = new SecTrust (id.Certificate, SecPolicy.CreateBasicX509Policy ()))
			using (var peer = new MCPeerID ("me")) {
				SecCertificate [] certs = new SecCertificate [trust.Count];
				for (int i=0; i < trust.Count; i++)
					certs [i] = trust [i];

				using (var s = new MCSession (peer, id, certs, MCEncryptionPreference.Required)) {
					Assert.AreSame (s.MyPeerID, peer, "MyPeerID");
					// it's a self-signed certificate that's used for the identity
					// so it's not added twice to the collection being returned
					Assert.That (s.SecurityIdentity.Count, Is.EqualTo (1), "SecurityIdentity");
					Assert.That (s.SecurityIdentity.GetItem<SecIdentity> (0).Handle, Is.EqualTo (certs [0].Handle), "SecurityIdentity");
					Assert.That (s.EncryptionPreference, Is.EqualTo (MCEncryptionPreference.Required), "EncryptionPreference");
					Assert.That (s.ConnectedPeers, Is.Empty, "ConnectedPeers");
				}
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
