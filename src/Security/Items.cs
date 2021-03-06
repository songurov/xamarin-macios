// 
// Items.cs: Implements the KeyChain query access APIs
//
// We use strong types and a helper SecQuery class to simplify the
// creation of the dictionary used to query the Keychain
// 
// Authors:
//	Miguel de Icaza
//	Sebastien Pouliot
//     
// Copyright 2010 Novell, Inc
// Copyright 2011-2016 Xamarin Inc
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
using System.Collections;
using System.Runtime.InteropServices;
using XamCore.CoreFoundation;
using XamCore.Foundation;
using XamCore.ObjCRuntime;
#if !MONOMAC
using XamCore.UIKit;
#endif

namespace XamCore.Security {

	public enum SecKind {
		InternetPassword,
		GenericPassword,
		Certificate,
		Key,
		Identity
	}

	public enum SecAccessible {
		Invalid = -1,
		WhenUnlocked,
		AfterFirstUnlock,
		Always,
		WhenUnlockedThisDeviceOnly,
		AfterFirstUnlockThisDeviceOnly,
		AlwaysThisDeviceOnly,
		WhenPasscodeSetThisDeviceOnly
	}

	public enum SecProtocol {
		Invalid = -1,
		Ftp, FtpAccount, Http, Irc, Nntp, Pop3, Smtp, Socks, Imap, Ldap, AppleTalk, Afp, Telnet, Ssh,
		Ftps, Https, HttpProxy, HttpsProxy, FtpProxy, Smb, Rtsp, RtspProxy, Daap, Eppc, Ipp,
		Nntps, Ldaps, Telnets, Imaps, Ircs, Pop3s, 
	}

	public enum SecAuthenticationType {
		Invalid = -1,
		Ntlm, Msn, Dpa, Rpa, HttpBasic, HttpDigest, HtmlForm, Default
	}

	public enum SecKeyClass {
		Invalid = -1,
		Public, Private, Symmetric
	}

	public enum SecKeyType {
		Invalid = -1,
		RSA, EC
	}

#if XAMCORE_2_0
	public class SecKeyChain : INativeObject {

		internal SecKeyChain (IntPtr handle)
		{
			Handle = handle;
		}

		public IntPtr Handle { get; internal set; }
#else
	public static class SecKeyChain {
#endif

		static NSNumber SetLimit (NSMutableDictionary dict, int max)
		{
			NSNumber n = null;
			IntPtr val;
			if (max == -1)
				val = SecMatchLimit.MatchLimitAll;
			else if (max == 1)
				val = SecMatchLimit.MatchLimitOne;
			else {
				n = NSNumber.FromInt32 (max);
				val = n.Handle;
			}
			
			dict.LowlevelSetObject (val, SecItem.MatchLimit);
			return n;
		}
		
		public static NSData QueryAsData (SecRecord query, bool wantPersistentReference, out SecStatusCode status)
		{
			if (query == null)
				throw new ArgumentNullException ("query");

			using (var copy = NSMutableDictionary.FromDictionary (query.queryDict)){
				SetLimit (copy, 1);
				copy.LowlevelSetObject (CFBoolean.True.Handle, SecItem.ReturnData);
				if (wantPersistentReference)
					copy.LowlevelSetObject (CFBoolean.True.Handle, SecItem.ReturnPersistentRef);
				
				IntPtr ptr;
				status = SecItem.SecItemCopyMatching (copy.Handle, out ptr);
				if (status == SecStatusCode.Success)
					return new NSData (ptr, false);
				return null;
			}
		}

		public static NSData [] QueryAsData (SecRecord query, bool wantPersistentReference, int max, out SecStatusCode status)
		{
			if (query == null)
				throw new ArgumentNullException ("query");

			using (var copy = NSMutableDictionary.FromDictionary (query.queryDict)){
				var n = SetLimit (copy, max);
				copy.LowlevelSetObject (CFBoolean.True.Handle, SecItem.ReturnData);
				if (wantPersistentReference)
					copy.LowlevelSetObject (CFBoolean.True.Handle, SecItem.ReturnPersistentRef);

				IntPtr ptr;
				status = SecItem.SecItemCopyMatching (copy.Handle, out ptr);
				n = null;
				if (status == SecStatusCode.Success){
					if (max == 1)
						return new NSData [] { new NSData (ptr, false) };

					var array = new NSArray (ptr);
					var records = new NSData [array.Count];
					for (uint i = 0; i < records.Length; i++)
						records [i] = new NSData (array.ValueAt (i), false);
					return records;
				}
				return null;
			}
		}
		
		public static NSData QueryAsData (SecRecord query)
		{
			SecStatusCode status;
			return QueryAsData (query, false, out status);
		}

		public static NSData [] QueryAsData (SecRecord query, int max)
		{
			SecStatusCode status;
			return QueryAsData (query, false, max, out status);
		}
		
		public static SecRecord QueryAsRecord (SecRecord query, out SecStatusCode result)
		{
			if (query == null)
				throw new ArgumentNullException ("query");
			
			using (var copy = NSMutableDictionary.FromDictionary (query.queryDict)){
				SetLimit (copy, 1);
				copy.LowlevelSetObject (CFBoolean.True.Handle, SecItem.ReturnAttributes);
				copy.LowlevelSetObject (CFBoolean.True.Handle, SecItem.ReturnData);
				IntPtr ptr;
				result = SecItem.SecItemCopyMatching (copy.Handle, out ptr);
				if (result == SecStatusCode.Success)
					return new SecRecord (new NSMutableDictionary (ptr, false));
				return null;
			}
		}
		
		public static SecRecord [] QueryAsRecord (SecRecord query, int max, out SecStatusCode result)
		{
			if (query == null)
				throw new ArgumentNullException ("query");
			
			using (var copy = NSMutableDictionary.FromDictionary (query.queryDict)){
				copy.LowlevelSetObject (CFBoolean.True.Handle, SecItem.ReturnAttributes);
				copy.LowlevelSetObject (CFBoolean.True.Handle, SecItem.ReturnData);
				var n = SetLimit (copy, max);
				
				IntPtr ptr;
				result = SecItem.SecItemCopyMatching (copy.Handle, out ptr);
				n = null;
				if (result == SecStatusCode.Success){
					var array = new NSArray (ptr);
					var records = new SecRecord [array.Count];
					for (uint i = 0; i < records.Length; i++)
						records [i] = new SecRecord (new NSMutableDictionary (array.ValueAt (i), false));
					return records;
				}
				return null;
			}
		}

		public static INativeObject[] QueryAsReference (SecRecord query, int max, out SecStatusCode result)
		{
			if (query == null){
				result = SecStatusCode.Param;
				return null;
			}

			using (var copy = NSMutableDictionary.FromDictionary (query.queryDict)){
				copy.LowlevelSetObject (CFBoolean.True.Handle, SecItem.ReturnRef);
				SetLimit (copy, max);

				IntPtr ptr;
				result = SecItem.SecItemCopyMatching (copy.Handle, out ptr);
				if ((result == SecStatusCode.Success) && (ptr != IntPtr.Zero)) {
					var array = NSArray.ArrayFromHandle<INativeObject> (ptr, p => {
						nint cfType = CFType.GetTypeID (p);
						if (cfType == SecCertificate.GetTypeID ())
							return new SecCertificate (p, true);
						else if (cfType == SecKey.GetTypeID ())
							return new SecKey (p, true);
						else if (cfType == SecIdentity.GetTypeID ())
							return new SecIdentity (p, true);
						else
							throw new Exception (String.Format ("Unexpected type: 0x{0:x}", cfType));
					});
					return array;
				}
				return null;
			}
		}

		public static SecStatusCode Add (SecRecord record)
		{
			if (record == null)
				throw new ArgumentNullException ("record");
			return SecItem.SecItemAdd (record.queryDict.Handle, IntPtr.Zero);
			
		}

		public static SecStatusCode Remove (SecRecord record)
		{
			if (record == null)
				throw new ArgumentNullException ("record");
			return SecItem.SecItemDelete (record.queryDict.Handle);
		}
		
		public static SecStatusCode Update (SecRecord query, SecRecord newAttributes)
		{
			if (query == null)
				throw new ArgumentNullException ("record");
			if (newAttributes == null)
				throw new ArgumentNullException ("newAttributes");

			return SecItem.SecItemUpdate (query.queryDict.Handle, newAttributes.queryDict.Handle);

		}
#if MONOMAC
		[DllImport (Constants.SecurityLibrary)]
		extern static SecStatusCode SecKeychainAddGenericPassword (
			IntPtr keychain,
			int serviceNameLength,
			IntPtr serviceName,
			int accountNameLength,
			IntPtr accountName,
			int passwordLength,
			IntPtr passwordData,
			IntPtr itemRef);

		[DllImport (Constants.SecurityLibrary)]
		extern static SecStatusCode SecKeychainFindGenericPassword (
			IntPtr keychainOrArray,
			int serviceNameLength,
			IntPtr serviceName,
			int accountNameLength,
			IntPtr accountName,
			out int passwordLength,
			out IntPtr passwordData,
			IntPtr itemRef);

		[DllImport (Constants.SecurityLibrary)]
		extern static SecStatusCode SecKeychainAddInternetPassword (
			IntPtr keychain,
			int serverNameLength,
			IntPtr serverName,
			int securityDomainLength,
			IntPtr securityDomain,
			int accountNameLength,
			IntPtr accountName,
			int pathLength,
			IntPtr path,
			short port,
			IntPtr protocol,
			IntPtr authenticationType,
			int passwordLength,
			IntPtr passwordData,
			IntPtr itemRef);

		[DllImport (Constants.SecurityLibrary)]
		extern static SecStatusCode SecKeychainFindInternetPassword (
			IntPtr keychain,
			int serverNameLength,
			IntPtr serverName,
			int securityDomainLength,
			IntPtr securityDomain,
			int accountNameLength,
			IntPtr accountName,
			int pathLength,
			IntPtr path,
			short port,
			IntPtr protocol,
			IntPtr authenticationType,
			out int passwordLength,
			out IntPtr passwordData,
			IntPtr itemRef);

		[DllImport (Constants.SecurityLibrary)]
		extern static SecStatusCode SecKeychainItemFreeContent (IntPtr attrList, IntPtr data);

		public static SecStatusCode AddInternetPassword (
			string serverName,
			string accountName,
			byte[] password,
			SecProtocol protocolType = SecProtocol.Http,
			short port = 0,
			string path = null,
			SecAuthenticationType authenticationType = SecAuthenticationType.Default,
			string securityDomain = null)
		{
			GCHandle serverHandle = new GCHandle ();
			GCHandle securityDomainHandle = new GCHandle ();
			GCHandle accountHandle = new GCHandle ();
			GCHandle pathHandle = new GCHandle ();
			GCHandle passwordHandle = new GCHandle ();
			
			int serverNameLength = 0;
			IntPtr serverNamePtr = IntPtr.Zero;
			int securityDomainLength = 0;
			IntPtr securityDomainPtr = IntPtr.Zero;
			int accountNameLength = 0;
			IntPtr accountNamePtr = IntPtr.Zero;
			int pathLength = 0;
			IntPtr pathPtr = IntPtr.Zero;
			int passwordLength = 0;
			IntPtr passwordPtr = IntPtr.Zero;
			
			try {
				
				if (!String.IsNullOrEmpty (serverName)) {
					var bytes = System.Text.Encoding.UTF8.GetBytes (serverName);
					serverNameLength = bytes.Length;
					serverHandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
					serverNamePtr = serverHandle.AddrOfPinnedObject ();
				}
				
				if (!String.IsNullOrEmpty (securityDomain)) {
					var bytes = System.Text.Encoding.UTF8.GetBytes (securityDomain);
					securityDomainLength = bytes.Length;
					securityDomainHandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
				}
				
				if (!String.IsNullOrEmpty (accountName)) {
					var bytes = System.Text.Encoding.UTF8.GetBytes (accountName);
					accountNameLength = bytes.Length;
					accountHandle = GCHandle.Alloc (bytes, GCHandleType.Pinned);
					accountNamePtr = accountHandle.AddrOfPinnedObject ();
				}
				
				if (!String.IsNullOrEmpty(path)) {
					var bytes = System.Text.Encoding.UTF8.GetBytes (path);
					pathLength = bytes.Length;
					pathHandle = GCHandle.Alloc (bytes, GCHandleType.Pinned);
					pathPtr = pathHandle.AddrOfPinnedObject ();
				}
				
				if (password != null && password.Length > 0) {
					passwordLength = password.Length;
					passwordHandle = GCHandle.Alloc (password, GCHandleType.Pinned);
					passwordPtr = passwordHandle.AddrOfPinnedObject ();
				}
				
				return SecKeychainAddInternetPassword (
					IntPtr.Zero,
					serverNameLength,
					serverNamePtr,
					securityDomainLength,
					securityDomainPtr,
					accountNameLength,
					accountNamePtr,
					pathLength,
					pathPtr,
					port,
					SecProtocolKeys.FromSecProtocol (protocolType),
					KeysAuthenticationType.FromSecAuthenticationType (authenticationType),
					passwordLength,
					passwordPtr,
					IntPtr.Zero);
			} finally {
				if (serverHandle.IsAllocated)
					serverHandle.Free ();
				if (accountHandle.IsAllocated)
					accountHandle.Free ();
				if (passwordHandle.IsAllocated)
					passwordHandle.Free ();
				if (securityDomainHandle.IsAllocated)
					securityDomainHandle.Free ();
				if (pathHandle.IsAllocated)
					pathHandle.Free ();
			}
		}
		
		
		public static SecStatusCode FindInternetPassword(
			string serverName,
			string accountName,
			out byte[] password,
			SecProtocol protocolType = SecProtocol.Http,
			short port = 0,
			string path = null,
			SecAuthenticationType authenticationType = SecAuthenticationType.Default,
			string securityDomain = null)
		{
			password = null;
			
			GCHandle serverHandle = new GCHandle ();
			GCHandle securityDomainHandle = new GCHandle ();
			GCHandle accountHandle = new GCHandle ();
			GCHandle pathHandle = new GCHandle ();
			
			int serverNameLength = 0;
			IntPtr serverNamePtr = IntPtr.Zero;
			int securityDomainLength = 0;
			IntPtr securityDomainPtr = IntPtr.Zero;
			int accountNameLength = 0;
			IntPtr accountNamePtr = IntPtr.Zero;
			int pathLength = 0;
			IntPtr pathPtr = IntPtr.Zero;
			IntPtr passwordPtr = IntPtr.Zero;
			
			try {
				if (!String.IsNullOrEmpty(serverName)) {
					var bytes = System.Text.Encoding.UTF8.GetBytes (serverName);
					serverNameLength = bytes.Length;
					serverHandle = GCHandle.Alloc (bytes, GCHandleType.Pinned);
					serverNamePtr = serverHandle.AddrOfPinnedObject ();
				}
				
				if (!String.IsNullOrEmpty(securityDomain)) {
					var bytes = System.Text.Encoding.UTF8.GetBytes (securityDomain);
					securityDomainLength = bytes.Length;
					securityDomainHandle = GCHandle.Alloc (bytes, GCHandleType.Pinned);
				}
				
				if (!String.IsNullOrEmpty(accountName)) {
					var bytes = System.Text.Encoding.UTF8.GetBytes (accountName);
					accountNameLength = bytes.Length;
					accountHandle = GCHandle.Alloc (bytes, GCHandleType.Pinned);
					accountNamePtr = accountHandle.AddrOfPinnedObject ();
				}
				
				if (!String.IsNullOrEmpty(path)) {
					var bytes = System.Text.Encoding.UTF8.GetBytes (path);
					pathLength = bytes.Length;
					pathHandle = GCHandle.Alloc (bytes, GCHandleType.Pinned);
					pathPtr = pathHandle.AddrOfPinnedObject ();
				}
				
				int passwordLength = 0;
				
				SecStatusCode code = SecKeychainFindInternetPassword(
					IntPtr.Zero,
					serverNameLength,
					serverNamePtr,
					securityDomainLength,
					securityDomainPtr,
					accountNameLength,
					accountNamePtr,
					pathLength,
					pathPtr,
					port,
					SecProtocolKeys.FromSecProtocol(protocolType),
					KeysAuthenticationType.FromSecAuthenticationType(authenticationType),
					out passwordLength,
					out passwordPtr,
					IntPtr.Zero);
				
				if (code == SecStatusCode.Success && passwordLength > 0) {
					password = new byte[passwordLength];
					Marshal.Copy(passwordPtr, password, 0, passwordLength);
				}
				
				return code;
				
			} finally {
				if (serverHandle.IsAllocated)
					serverHandle.Free();
				if (accountHandle.IsAllocated)
					accountHandle.Free();
				if (securityDomainHandle.IsAllocated)
					securityDomainHandle.Free();
				if (pathHandle.IsAllocated)
					pathHandle.Free();
				if (passwordPtr != IntPtr.Zero)
					SecKeychainItemFreeContent(IntPtr.Zero, passwordPtr);
			}
		}

		public static SecStatusCode AddGenericPassword (string serviceName, string accountName, byte[] password)
		{
			GCHandle serviceHandle = new GCHandle ();
			GCHandle accountHandle = new GCHandle ();
			GCHandle passwordHandle = new GCHandle ();
			
			int serviceNameLength = 0;
			IntPtr serviceNamePtr = IntPtr.Zero;
			int accountNameLength = 0;
			IntPtr accountNamePtr = IntPtr.Zero;
			int passwordLength = 0;
			IntPtr passwordPtr = IntPtr.Zero;
			
			try {
				if (!String.IsNullOrEmpty(serviceName)) {
					var bytes = System.Text.Encoding.UTF8.GetBytes (serviceName);
					serviceNameLength = bytes.Length;
					serviceHandle = GCHandle.Alloc (bytes, GCHandleType.Pinned);
					serviceNamePtr = serviceHandle.AddrOfPinnedObject ();
				}
				
				if (!String.IsNullOrEmpty(accountName)) {
					var bytes = System.Text.Encoding.UTF8.GetBytes (accountName);
					accountNameLength = bytes.Length;
					accountHandle = GCHandle.Alloc (bytes, GCHandleType.Pinned);
					accountNamePtr = accountHandle.AddrOfPinnedObject ();
				}

				if (password != null && password.Length > 0) {
					passwordLength = password.Length;
					passwordHandle = GCHandle.Alloc (password, GCHandleType.Pinned);
					passwordPtr = passwordHandle.AddrOfPinnedObject ();
				}

				return SecKeychainAddGenericPassword(
					IntPtr.Zero,
					serviceNameLength,
					serviceNamePtr,
					accountNameLength,
					accountNamePtr,
					passwordLength,
					passwordPtr,
					IntPtr.Zero
					);

			} finally {
				if (serviceHandle.IsAllocated)
					serviceHandle.Free();
				if (accountHandle.IsAllocated)
					accountHandle.Free();
				if (passwordHandle.IsAllocated)
					passwordHandle.Free();
			}
		}

		public static SecStatusCode FindGenericPassword(string serviceName, string accountName, out byte[] password)
		{
			password = null;

			GCHandle serviceHandle = new GCHandle ();
			GCHandle accountHandle = new GCHandle ();
			
			int serviceNameLength = 0;
			IntPtr serviceNamePtr = IntPtr.Zero;
			int accountNameLength = 0;
			IntPtr accountNamePtr = IntPtr.Zero;
			IntPtr passwordPtr = IntPtr.Zero;
			
			try {
				
				if (!String.IsNullOrEmpty(serviceName)) {
					var bytes = System.Text.Encoding.UTF8.GetBytes (serviceName);
					serviceNameLength = bytes.Length;
					serviceHandle = GCHandle.Alloc (bytes, GCHandleType.Pinned);
					serviceNamePtr = serviceHandle.AddrOfPinnedObject();
				}
				
				if (!String.IsNullOrEmpty(accountName)) {
					var bytes = System.Text.Encoding.UTF8.GetBytes (accountName);
					accountNameLength = bytes.Length;
					accountHandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
					accountNamePtr = accountHandle.AddrOfPinnedObject();
				}
				
				int passwordLength = 0;
				
				var code = SecKeychainFindGenericPassword(
					IntPtr.Zero,
					serviceNameLength,
					serviceNamePtr,
					accountNameLength,
					accountNamePtr,
					out passwordLength,
					out passwordPtr,
					IntPtr.Zero
					);
				
				if (code == SecStatusCode.Success && passwordLength > 0){
					password = new byte[passwordLength];
					Marshal.Copy(passwordPtr, password, 0, passwordLength);
				}
				
				return code;
				
			} finally {
				if (serviceHandle.IsAllocated)
					serviceHandle.Free();
				if (accountHandle.IsAllocated)
					accountHandle.Free();
				if (passwordPtr != IntPtr.Zero)
					SecKeychainItemFreeContent(IntPtr.Zero, passwordPtr);
			}
		}
#else
		public static object QueryAsConcreteType (SecRecord query, out SecStatusCode result)
		{
			if (query == null){
				result = SecStatusCode.Param;
				return null;
			}
			
			using (var copy = NSMutableDictionary.FromDictionary (query.queryDict)){
				copy.LowlevelSetObject (CFBoolean.True.Handle, SecItem.ReturnRef);
				SetLimit (copy, 1);
				
				IntPtr ptr;
				result = SecItem.SecItemCopyMatching (copy.Handle, out ptr);
				if ((result == SecStatusCode.Success) && (ptr != IntPtr.Zero)) {
					nint cfType = CFType.GetTypeID (ptr);
					
					if (cfType == SecCertificate.GetTypeID ())
						return new SecCertificate (ptr, true);
					else if (cfType == SecKey.GetTypeID ())
						return new SecKey (ptr, true);
					else if (cfType == SecIdentity.GetTypeID ())
						return new SecIdentity (ptr, true);
					else
						throw new Exception (String.Format ("Unexpected type: 0x{0:x}", cfType));
				} 
				return null;
			}
		}
#endif

		public static void AddIdentity (SecIdentity identity)
		{
			if (identity == null)
				throw new ArgumentNullException ("identity");
			using (var record = new SecRecord ()) {
				record.SetValueRef (identity);

				SecStatusCode result = SecKeyChain.Add (record);

				if (result != SecStatusCode.DuplicateItem && result != SecStatusCode.Success)
					throw new InvalidOperationException (result.ToString ());
			}
		}

		public static void RemoveIdentity (SecIdentity identity)
		{
			if (identity == null)
				throw new ArgumentNullException ("identity");
			using (var record = new SecRecord ()) {
				record.SetValueRef (identity);

				SecStatusCode result = SecKeyChain.Remove (record);

				if (result != SecStatusCode.ItemNotFound && result != SecStatusCode.Success)
					throw new InvalidOperationException (result.ToString ());
			}
		}

		public static SecIdentity FindIdentity (SecCertificate certificate, bool throwOnError = false)
		{
			if (certificate == null)
				throw new ArgumentNullException ("certificate");
			var identity = FindIdentity (cert => SecCertificate.Equals (certificate, cert));
			if (!throwOnError || identity != null)
				return identity;

			throw new InvalidOperationException (string.Format ("Could not find SecIdentity for certificate '{0}' in keychain.", certificate.SubjectSummary));
		}

		static SecIdentity FindIdentity (Predicate<SecCertificate> filter)
		{
			/*
			 * Unfortunately, SecItemCopyMatching() does not allow any search
			 * filters when looking up an identity.
			 * 
			 * The following lookup will return all identities from the keychain -
			 * we then need need to find the right one.
			 */
			using (var record = new SecRecord (SecKind.Identity)) {
				SecStatusCode status;
				var result = SecKeyChain.QueryAsReference (record, -1, out status);
				if (status != SecStatusCode.Success || result == null)
					return null;

				for (int i = 0; i < result.Length; i++) {
					var identity = (SecIdentity)result [i];
					if (filter (identity.Certificate))
						return identity;
				}
			}

			return null;
		}
	}
	
	public class SecRecord : IDisposable {
		// Fix <= iOS 6 Behaviour - Desk #83099
		// NSCFDictionary: mutating method sent to immutable object
		// iOS 6 returns an inmutable NSDictionary handle and when we try to set its values it goes kaboom
		// By explicitly calling `MutableCopy` we ensure we always have a mutable reference we expect that.
		NSMutableDictionary _queryDict;
		internal NSMutableDictionary queryDict 
		{ 
			get {
				return _queryDict;
			}
			set {
				_queryDict = value != null ? (NSMutableDictionary)value.MutableCopy () : null;
			}
		}

		internal SecRecord (NSMutableDictionary dict)
		{
			queryDict = dict;
		}

		// it's possible to query something without a class
		public SecRecord ()
		{
			queryDict = new NSMutableDictionary ();
		}

		public SecRecord (SecKind secKind)
		{
			var kind = SecClass.FromSecKind (secKind);
#if MONOMAC
			queryDict = NSMutableDictionary.LowlevelFromObjectAndKey (kind, SecClass.SecClassKey);
#elif WATCH
			queryDict = NSMutableDictionary.LowlevelFromObjectAndKey (kind, SecClass.SecClassKey);
#else
			// Apple changed/fixed this in iOS7 (not the only change, see comments above)
			// test suite has a test case that needs to work on both pre-7.0 and post-7.0
			if ((kind == SecClass.Identity) && !UIDevice.CurrentDevice.CheckSystemVersion (7,0))
				queryDict = new NSMutableDictionary ();
			else
				queryDict = NSMutableDictionary.LowlevelFromObjectAndKey (kind, SecClass.SecClassKey);
#endif
		}

		public SecRecord Clone ()
		{
			return new SecRecord (NSMutableDictionary.FromDictionary (queryDict));
		}

		// some API are unusable without this (e.g. SecKey.GenerateKeyPair) without duplicating much of SecRecord logic
		public NSDictionary ToDictionary ()
		{
			return queryDict;
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

#if XAMCORE_2_0
		protected virtual void Dispose (bool disposing)
#else
		public virtual void Dispose (bool disposing)
#endif
		{
			if (queryDict != null){
				if (disposing){
					queryDict.Dispose ();
					queryDict = null;
				}
			}
		}

		~SecRecord ()
		{
			Dispose (false);
		}
			
		IntPtr Fetch (IntPtr key)
		{
			return queryDict.LowlevelObjectForKey (key);
		}

		NSObject FetchObject (IntPtr key)
		{
			return Runtime.GetNSObject<NSObject> (Fetch (key));
		}

		string FetchString (IntPtr key)
		{
			return NSString.FromHandle (Fetch (key));
		}

		int FetchInt (IntPtr key)
		{
			var obj = Runtime.GetNSObject<NSNumber> (Fetch (key));
			return obj == null ? -1 : obj.Int32Value;
		}

		bool FetchBool (IntPtr key, bool defaultValue)
		{
			var obj = Runtime.GetNSObject<NSNumber> (Fetch (key));
			return obj == null ? defaultValue : obj.Int32Value != 0;
		}

		T Fetch<T> (IntPtr key) where T : NSObject
		{
			return Runtime.GetNSObject<T> (Fetch (key));
		}
		

		void SetValue (NSObject val, IntPtr key)
		{
			queryDict.LowlevelSetObject (val, key);
		}

		void SetValue (IntPtr val, IntPtr key)
		{
			queryDict.LowlevelSetObject (val, key);
		}

		void SetValue (string value, IntPtr key)
		{
			// FIXME: it's not clear that we should not allow null (i.e. that null should remove entries)
			// but this is compatible with the exiting behaviour of older XI/XM
			if (value == null)
				throw new ArgumentNullException (nameof (value));
			var ptr = NSString.CreateNative (value);
			queryDict.LowlevelSetObject (ptr, key);
			NSString.ReleaseNative (ptr);
		}
		
		//
		// Attributes
		//
		public SecAccessible Accessible {
			get {
				return KeysAccessible.ToSecAccessible (Fetch (SecAttributeKey.Accessible));
			}
			
			set {
				SetValue (KeysAccessible.FromSecAccessible (value), SecAttributeKey.Accessible);
			}
		}

		public bool Synchronizable {
			get {
				return FetchBool (SecAttributeKey.Synchronizable, false);
			}
			set {
				SetValue (new NSNumber (value ? 1 : 0), SecAttributeKey.Synchronizable);
			}
		}

		public bool SynchronizableAny {
			get {
				return FetchBool (SecAttributeKey.SynchronizableAny, false);
			}
			set {
				SetValue (new NSNumber (value ? 1 : 0), SecAttributeKey.SynchronizableAny);
			}
		}

#if !MONOMAC
		[iOS (9,0)]
		public string SyncViewHint {
			get {
				return FetchString (SecAttributeKey.SyncViewHint);
			}
			set {
				SetValue (value, SecAttributeKey.SyncViewHint);
			}
		}

		[iOS (9,0)]
		public SecTokenID TokenID {
			get {
				return SecTokenIDExtensions.GetValue (Fetch<NSString> (SecAttributeKey.TokenID));
			}
			set {
				// choose wisely to avoid NSString -> string -> NSString conversion
				SetValue ((NSObject) value.GetConstant (), SecAttributeKey.TokenID);
			}
		}
#endif

		public NSDate CreationDate {
			get {
				return (NSDate) FetchObject (SecAttributeKey.CreationDate);
			}
			
			set {
				if (value == null)
					throw new ArgumentNullException ("value");
				SetValue (value, SecAttributeKey.CreationDate);
			}
		}

		public NSDate ModificationDate {
			get {
				return (NSDate) FetchObject (SecAttributeKey.ModificationDate);
			}
			
			set {
				if (value == null)
					throw new ArgumentNullException ("value");
				SetValue (value, SecAttributeKey.ModificationDate);
			}
		}

		public string Description {
			get {
				return FetchString (SecAttributeKey.Description);
			}

			set {
				SetValue (value, SecAttributeKey.Description);
			}
		}

		public string Comment {
			get {
				return FetchString (SecAttributeKey.Comment);
			}

			set {
				SetValue (value, SecAttributeKey.Comment);
			}
		}

		public int Creator {
			get {
				return FetchInt (SecAttributeKey.Creator);
			}
					
			set {
				SetValue (new NSNumber (value), SecAttributeKey.Creator);
			}
		}

		public int CreatorType {
			get {
				return FetchInt (SecAttributeKey.Type);
			}
					
			set {
				SetValue (new NSNumber (value), SecAttributeKey.Type);
			}
		}

		public string Label {
			get {
				return FetchString (SecAttributeKey.Label);
			}

			set {
				SetValue (value, SecAttributeKey.Label);
			}
		}

		public bool Invisible {
			get {
				return Fetch (SecAttributeKey.IsInvisible) == CFBoolean.True.Handle;
			}
			
			set {
				SetValue (CFBoolean.FromBoolean (value).Handle, SecAttributeKey.IsInvisible);
			}
		}

		public bool IsNegative {
			get {
				return Fetch (SecAttributeKey.IsNegative) == CFBoolean.True.Handle;
			}
			
			set {
				SetValue (CFBoolean.FromBoolean (value).Handle, SecAttributeKey.IsNegative);
			}
		}

		public string Account {
			get {
				return FetchString (SecAttributeKey.Account);
			}

			set {
				SetValue (value, SecAttributeKey.Account);
			}
		}

		public string Service {
			get {
				return FetchString (SecAttributeKey.Service);
			}

			set {
				SetValue (value, SecAttributeKey.Service);
			}
		}

#if !MONOMAC || !XAMCORE_2_0
		public string UseOperationPrompt {
			get {
				return FetchString (SecItem.UseOperationPrompt);
			}
			set {
				SetValue (value, SecItem.UseOperationPrompt);
			}
		}

		[Availability (Introduced = Platform.iOS_8_0, Deprecated = Platform.iOS_9_0, Message = "Use AuthenticationUI property")]
		public bool UseNoAuthenticationUI {
			get {
				return Fetch (SecItem.UseNoAuthenticationUI) == CFBoolean.True.Handle;
			}
			set {
				SetValue (CFBoolean.FromBoolean (value).Handle, SecItem.UseNoAuthenticationUI);
			}
		}
#endif
		[iOS (9,0)][Mac (10,11)]
		public SecAuthenticationUI AuthenticationUI {
			get {
				var s = Fetch<NSString> (SecItem.UseAuthenticationUI);
				return s == null ? SecAuthenticationUI.NotSet : SecAuthenticationUIExtensions.GetValue (s);
			}
			set {
				SetValue ((NSObject) value.GetConstant (), SecItem.UseAuthenticationUI);
			}
		}

		// Must store the _secAccessControl here, since we have no way of inspecting its values if
		// it is ever returned from a dictionary, so return what we cached.
		SecAccessControl _secAccessControl;
		public SecAccessControl AccessControl {
			get {
				return _secAccessControl;
			}
			set {
				if (value == null)
					throw new ArgumentNullException ("value");
				_secAccessControl = value;
				SetValue (value.Handle, SecAttributeKey.AccessControl);
			}
		}

		public NSData Generic {
			get {
				return Fetch<NSData> (SecAttributeKey.Generic);
			}

			set {
				if (value == null)
					throw new ArgumentNullException ("value");
				SetValue (value, SecAttributeKey.Generic);
			}
		}

		public string SecurityDomain {
			get {
				return FetchString (SecAttributeKey.SecurityDomain);
			}

			set {
				SetValue (value, SecAttributeKey.SecurityDomain);
			}
		}

		public string Server {
			get {
				return FetchString (SecAttributeKey.Server);
			}

			set {
				SetValue (value, SecAttributeKey.Server);
			}
		}

		public SecProtocol Protocol {
			get {
				return SecProtocolKeys.ToSecProtocol (Fetch (SecAttributeKey.Protocol));
			}
			
			set {
				SetValue (SecProtocolKeys.FromSecProtocol (value), SecAttributeKey.Protocol);
			}
		}

		public SecAuthenticationType AuthenticationType {
			get {
				var at = Fetch (SecAttributeKey.AuthenticationType);
				if (at == IntPtr.Zero)
					return SecAuthenticationType.Default;
				return KeysAuthenticationType.ToSecAuthenticationType (at);
			}
			
			set {
				SetValue (KeysAuthenticationType.FromSecAuthenticationType (value),
							     SecAttributeKey.AuthenticationType);
			}
		}

		public int Port {
			get {
				return FetchInt (SecAttributeKey.Port);
			}
					
			set {
				SetValue (new NSNumber (value), SecAttributeKey.Port);
			}
		}

		public string Path {
			get {
				return FetchString (SecAttributeKey.Path);
			}

			set {
				SetValue (value, SecAttributeKey.Path);
			}
		}

		// read only
		public string Subject {
			get {
				return FetchString (SecAttributeKey.Subject);
			}
		}

		// read only
		public NSData Issuer {
			get {
				return Fetch<NSData> (SecAttributeKey.Issuer);
			}
		}

		// read only
		public NSData SerialNumber {
			get {
				return Fetch<NSData> (SecAttributeKey.SerialNumber);
			}
		}

		// read only
		public NSData SubjectKeyID {
			get {
				return Fetch<NSData> (SecAttributeKey.SubjectKeyID);
			}
		}

		// read only
		public NSData PublicKeyHash {
			get {
				return Fetch<NSData> (SecAttributeKey.PublicKeyHash);
			}
		}

		// read only
		public NSNumber CertificateType {
			get {
				return Fetch<NSNumber> (SecAttributeKey.CertificateType);
			}
		}

		// read only
		public NSNumber CertificateEncoding {
			get {
				return Fetch<NSNumber> (SecAttributeKey.CertificateEncoding);
			}
		}

		public SecKeyClass KeyClass {
			get {
				var k = Fetch (SecAttributeKey.KeyClass);
				if (k == IntPtr.Zero)
					return SecKeyClass.Invalid;
				if (CFType.Equal (k, ClassKeys.Public))
					return SecKeyClass.Public;
				else if (CFType.Equal (k, ClassKeys.Private))
					return SecKeyClass.Private;
				else if (CFType.Equal (k, ClassKeys.Symmetric))
					return SecKeyClass.Symmetric;
				else
					return SecKeyClass.Invalid;
			}
			set {
				SetValue (value == SecKeyClass.Public ? ClassKeys.Public : value == SecKeyClass.Private ? ClassKeys.Private : ClassKeys.Symmetric, SecAttributeKey.KeyClass);
			}
		}

		public string ApplicationLabel {
			get {
				return FetchString (SecAttributeKey.ApplicationLabel);
			}

			set {
				SetValue (value, SecAttributeKey.ApplicationLabel);
			}
		}

		public bool IsPermanent {
			get {
				return Fetch (SecAttributeKey.IsPermanent) == CFBoolean.True.Handle;
			}
			
			set {
				SetValue (CFBoolean.FromBoolean (value).Handle, SecAttributeKey.IsPermanent);
			}
		}

		public NSData ApplicationTag {
			get {
				return Fetch<NSData> (SecAttributeKey.ApplicationTag);
			}
			
			set {
				if (value == null)
					throw new ArgumentNullException ("value");
				SetValue (value, SecAttributeKey.ApplicationTag);
			}
		}

		public SecKeyType KeyType {
			get {
				var k = Fetch (SecAttributeKey.KeyType);
				if (k == IntPtr.Zero)
					return SecKeyType.Invalid;
				if (CFType.Equal (k, KeyTypeKeys.RSA))
					return SecKeyType.RSA;
				else if (CFType.Equal (k, KeyTypeKeys.EC))
					return SecKeyType.EC;
				else
					return SecKeyType.Invalid;
			}
			
			set {
				SetValue (value == SecKeyType.RSA ? KeyTypeKeys.RSA : KeyTypeKeys.EC, SecAttributeKey.KeyType);
			}
		}

		public int KeySizeInBits {
			get {
				return FetchInt (SecAttributeKey.KeySizeInBits);
			}
					
			set {
				SetValue (new NSNumber (value), SecAttributeKey.KeySizeInBits);
			}
		}

		public int EffectiveKeySize {
			get {
				return FetchInt (SecAttributeKey.EffectiveKeySize);
			}
					
			set {
				SetValue (new NSNumber (value), SecAttributeKey.EffectiveKeySize);
			}
		}

		public bool CanEncrypt {
			get {
				return Fetch (SecAttributeKey.CanEncrypt) == CFBoolean.True.Handle;
			}
			
			set {
				SetValue (CFBoolean.FromBoolean (value).Handle, SecAttributeKey.CanEncrypt);
			}
		}

		public bool CanDecrypt {
			get {
				return Fetch (SecAttributeKey.CanDecrypt) == CFBoolean.True.Handle;
			}
			
			set {
				SetValue (CFBoolean.FromBoolean (value).Handle, SecAttributeKey.CanDecrypt);
			}
		}

		public bool CanDerive {
			get {
				return Fetch (SecAttributeKey.CanDerive) == CFBoolean.True.Handle;
			}
			
			set {
				SetValue (CFBoolean.FromBoolean (value).Handle, SecAttributeKey.CanDerive);
			}
		}

		public bool CanSign {
			get {
				return Fetch (SecAttributeKey.CanSign) == CFBoolean.True.Handle;
			}
			
			set {
				SetValue (CFBoolean.FromBoolean (value).Handle, SecAttributeKey.CanSign);
			}
		}

		public bool CanVerify {
			get {
				return Fetch (SecAttributeKey.CanVerify) == CFBoolean.True.Handle;
			}
			
			set {
				SetValue (CFBoolean.FromBoolean (value).Handle, SecAttributeKey.CanVerify);
			}
		}

		public bool CanWrap {
			get {
				return Fetch (SecAttributeKey.CanWrap) == CFBoolean.True.Handle;
			}
			
			set {
				SetValue (CFBoolean.FromBoolean (value).Handle, SecAttributeKey.CanWrap);
			}
		}

		public bool CanUnwrap {
			get {
				return Fetch (SecAttributeKey.CanUnwrap) == CFBoolean.True.Handle;
			}
			
			set {
				SetValue (CFBoolean.FromBoolean (value).Handle, SecAttributeKey.CanUnwrap);
			}
		}

		public string AccessGroup {
			get {
				return FetchString (SecAttributeKey.AccessGroup);
			}

			set {
				SetValue (value, SecAttributeKey.AccessGroup);
			}
		}

		//
		// Matches
		//

		public SecPolicy MatchPolicy {
			get {
				var pol = Fetch (SecItem.MatchPolicy);
				return (pol == IntPtr.Zero) ? null : new SecPolicy (pol);
			}

			set {
				if (value == null)
					throw new ArgumentNullException ("value");
				SetValue (value.Handle, SecItem.MatchPolicy);
			}
		}

#if XAMCORE_2_0
		public SecKeyChain[] MatchItemList {
			get {
				return NSArray.ArrayFromHandle<SecKeyChain> (Fetch (SecItem.MatchItemList));
			}

			set {
				if (value == null)
					throw new ArgumentNullException ("value");
				using (var array = NSArray.FromNativeObjects (value))
					SetValue (array, SecItem.MatchItemList);
			}
		}
#else
		public NSArray MatchItemList {
			get {
				return (NSArray) Runtime.GetNSObject (Fetch (SecItem.MatchItemList));
			}

			set {
				if (value == null)
					throw new ArgumentNullException ("value");
				SetValue (value, SecItem.MatchItemList);
			}
		}
#endif

		public NSData [] MatchIssuers {
			get {
				return NSArray.ArrayFromHandle<NSData> (Fetch (SecItem.MatchIssuers));
			}
			set {
				if (value == null)
					throw new ArgumentNullException ("value");
				
				SetValue (NSArray.FromNSObjects (value), SecItem.MatchIssuers);
			}
		}

		public string MatchEmailAddressIfPresent {
			get {
				return FetchString (SecItem.MatchEmailAddressIfPresent);
			}

			set {
				SetValue (value, SecItem.MatchEmailAddressIfPresent);
			}
		}

		public string MatchSubjectContains {
			get {
				return FetchString (SecItem.MatchSubjectContains);
			}

			set {
				SetValue (value, SecItem.MatchSubjectContains);
			}
		}

		public bool MatchCaseInsensitive {
			get {
				return Fetch (SecItem.MatchCaseInsensitive) == CFBoolean.True.Handle;
			}
			
			set {
				SetValue (CFBoolean.FromBoolean (value).Handle, SecItem.MatchCaseInsensitive);
			}
		}

		public bool MatchTrustedOnly {
			get {
				return Fetch (SecItem.MatchTrustedOnly) == CFBoolean.True.Handle;
			}
			
			set {
				SetValue (CFBoolean.FromBoolean (value).Handle, SecItem.MatchTrustedOnly);
			}
		}

		public NSDate MatchValidOnDate {
			get {
				return (NSDate) (Runtime.GetNSObject (Fetch (SecItem.MatchValidOnDate)));
			}
			
			set {
				if (value == null)
					throw new ArgumentNullException ("value");
				SetValue (value, SecItem.MatchValidOnDate);
			}
		}

		public NSData ValueData {
			get {
				return Fetch<NSData> (SecItem.ValueData);
			}

			set {
				if (value == null)
					throw new ArgumentNullException ("value");
				SetValue (value, SecItem.ValueData);
			}
		}

#if !XAMCORE_2_0
		[Obsolete ("Use GetValueRef<T> and SetValueRef<T> instead")]
		public NSObject ValueRef {
			get {
				return FetchObject (SecItem.ValueRef);
			}

			set {
				if (value == null)
					throw new ArgumentNullException ("value");
				SetValue (value, SecItem.ValueRef);
			}
		}
#endif
			
		public T GetValueRef<T> () where T : class, INativeObject
		{
			return Runtime.GetINativeObject<T> (queryDict.LowlevelObjectForKey (SecItem.ValueRef), false);
		}

		// This can be used to store SecKey, SecCertificate, SecIdentity and SecKeyChainItem (not bound yet, and not availble on iOS)
		public void SetValueRef (INativeObject value)
		{
			SetValue (value == null ? IntPtr.Zero : value.Handle, SecItem.ValueRef);
		}
	}
	
	internal partial class SecItem {

		[DllImport (Constants.SecurityLibrary)]
		internal extern static SecStatusCode SecItemCopyMatching (/* CFDictionaryRef */ IntPtr query, /* CFTypeRef* */ out IntPtr result);

		[DllImport (Constants.SecurityLibrary)]
		internal extern static SecStatusCode SecItemAdd (/* CFDictionaryRef */ IntPtr attributes, /* CFTypeRef* */ IntPtr result);

		[DllImport (Constants.SecurityLibrary)]
		internal extern static SecStatusCode SecItemDelete (/* CFDictionaryRef */ IntPtr query);

		[DllImport (Constants.SecurityLibrary)]
		internal extern static SecStatusCode SecItemUpdate (/* CFDictionaryRef */ IntPtr query, /* CFDictionaryRef */ IntPtr attributesToUpdate);
	}

	internal static partial class SecClass {
	
		public static IntPtr FromSecKind (SecKind secKind)
		{
			switch (secKind){
			case SecKind.InternetPassword:
				return InternetPassword;
			case SecKind.GenericPassword:
				return GenericPassword;
			case SecKind.Certificate:
				return Certificate;
			case SecKind.Key:
				return Key;
			case SecKind.Identity:
				return Identity;
			default:
				throw new ArgumentException ("secKind");
			}
		}
	}
	
	internal static partial class KeysAccessible {
		public static IntPtr FromSecAccessible (SecAccessible accessible)
		{
			switch (accessible){
			case SecAccessible.WhenUnlocked:
				return WhenUnlocked;
			case SecAccessible.AfterFirstUnlock:
				return AfterFirstUnlock;
			case SecAccessible.Always:
				return Always;
			case SecAccessible.WhenUnlockedThisDeviceOnly:
				return WhenUnlockedThisDeviceOnly;
			case SecAccessible.AfterFirstUnlockThisDeviceOnly:
				return AfterFirstUnlockThisDeviceOnly;
			case SecAccessible.AlwaysThisDeviceOnly:
				return AlwaysThisDeviceOnly;
			case SecAccessible.WhenPasscodeSetThisDeviceOnly:
				return WhenPasscodeSetThisDeviceOnly;
			default:
				throw new ArgumentException ("accessible");
			}
		}
			
		// note: we're comparing pointers - but it's an (even if opaque) CFType
		// and it turns out to be using CFString - so we need to use CFTypeEqual
		public static SecAccessible ToSecAccessible (IntPtr handle)
		{
			if (handle == IntPtr.Zero)
				return SecAccessible.Invalid;
			if (CFType.Equal (handle, WhenUnlocked))
				return SecAccessible.WhenUnlocked;
			if (CFType.Equal (handle, AfterFirstUnlock))
				return SecAccessible.AfterFirstUnlock;
			if (CFType.Equal (handle, Always))
				return SecAccessible.Always;
			if (CFType.Equal (handle, WhenUnlockedThisDeviceOnly))
				return SecAccessible.WhenUnlockedThisDeviceOnly;
			if (CFType.Equal (handle, AfterFirstUnlockThisDeviceOnly))
				return SecAccessible.AfterFirstUnlockThisDeviceOnly;
			if (CFType.Equal (handle, AlwaysThisDeviceOnly))
				return SecAccessible.AlwaysThisDeviceOnly;
			if (CFType.Equal (handle, WhenUnlockedThisDeviceOnly))
				return SecAccessible.WhenUnlockedThisDeviceOnly;
			return SecAccessible.Invalid;
		}
	}
	
	internal static partial class SecProtocolKeys {
		public static IntPtr FromSecProtocol (SecProtocol protocol)
		{
			switch (protocol){
			case SecProtocol.Ftp: return FTP;
			case SecProtocol.FtpAccount: return FTPAccount;
			case SecProtocol.Http: return HTTP;
			case SecProtocol.Irc: return IRC;
			case SecProtocol.Nntp: return NNTP;
			case SecProtocol.Pop3: return POP3;
			case SecProtocol.Smtp: return SMTP;
			case SecProtocol.Socks:return SOCKS;
			case SecProtocol.Imap:return IMAP;
			case SecProtocol.Ldap:return LDAP;
			case SecProtocol.AppleTalk:return AppleTalk;
			case SecProtocol.Afp:return AFP;
			case SecProtocol.Telnet:return Telnet;
			case SecProtocol.Ssh:return SSH;
			case SecProtocol.Ftps:return FTPS;
			case SecProtocol.Https:return HTTPS;
			case SecProtocol.HttpProxy:return HTTPProxy;
			case SecProtocol.HttpsProxy:return HTTPSProxy;
			case SecProtocol.FtpProxy:return FTPProxy;
			case SecProtocol.Smb:return SMB;
			case SecProtocol.Rtsp:return RTSP;
			case SecProtocol.RtspProxy:return RTSPProxy;
			case SecProtocol.Daap:return DAAP;
			case SecProtocol.Eppc:return EPPC;
			case SecProtocol.Ipp:return IPP;
			case SecProtocol.Nntps:return NNTPS;
			case SecProtocol.Ldaps:return LDAPS;
			case SecProtocol.Telnets:return TelnetS;
			case SecProtocol.Imaps:return IMAPS;
			case SecProtocol.Ircs:return IRCS;
			case SecProtocol.Pop3s: return POP3S;
			}
			throw new ArgumentException ("protocol");
		}

		public static SecProtocol ToSecProtocol (IntPtr handle)
		{
			if (handle == IntPtr.Zero)
				return SecProtocol.Invalid;
			if (CFType.Equal (handle, FTP))
				return SecProtocol.Ftp;
			if (CFType.Equal (handle, FTPAccount))
				return SecProtocol.FtpAccount;
			if (CFType.Equal (handle, HTTP))
				return SecProtocol.Http;
			if (CFType.Equal (handle, IRC))
				return SecProtocol.Irc;
			if (CFType.Equal (handle, NNTP))
				return SecProtocol.Nntp;
			if (CFType.Equal (handle, POP3))
				return SecProtocol.Pop3;
			if (CFType.Equal (handle, SMTP))
				return SecProtocol.Smtp;
			if (CFType.Equal (handle, SOCKS))
				return SecProtocol.Socks;
			if (CFType.Equal (handle, IMAP))
				return SecProtocol.Imap;
			if (CFType.Equal (handle, LDAP))
				return SecProtocol.Ldap;
			if (CFType.Equal (handle, AppleTalk))
				return SecProtocol.AppleTalk;
			if (CFType.Equal (handle, AFP))
				return SecProtocol.Afp;
			if (CFType.Equal (handle, Telnet))
				return SecProtocol.Telnet;
			if (CFType.Equal (handle, SSH))
				return SecProtocol.Ssh;
			if (CFType.Equal (handle, FTPS))
				return SecProtocol.Ftps;
			if (CFType.Equal (handle, HTTPS))
				return SecProtocol.Https;
			if (CFType.Equal (handle, HTTPProxy))
				return SecProtocol.HttpProxy;
			if (CFType.Equal (handle, HTTPSProxy))
				return SecProtocol.HttpsProxy;
			if (CFType.Equal (handle, FTPProxy))
				return SecProtocol.FtpProxy;
			if (CFType.Equal (handle, SMB))
				return SecProtocol.Smb;
			if (CFType.Equal (handle, RTSP))
				return SecProtocol.Rtsp;
			if (CFType.Equal (handle, RTSPProxy))
				return SecProtocol.RtspProxy;
			if (CFType.Equal (handle, DAAP))
				return SecProtocol.Daap;
			if (CFType.Equal (handle, EPPC))
				return SecProtocol.Eppc;
			if (CFType.Equal (handle, IPP))
				return SecProtocol.Ipp;
			if (CFType.Equal (handle, NNTPS))
				return SecProtocol.Nntps;
			if (CFType.Equal (handle, LDAPS))
				return SecProtocol.Ldaps;
			if (CFType.Equal (handle, TelnetS))
				return SecProtocol.Telnets;
			if (CFType.Equal (handle, IMAPS))
				return SecProtocol.Imaps;
			if (CFType.Equal (handle, IRCS))
				return SecProtocol.Ircs;
			if (CFType.Equal (handle, POP3S))
				return SecProtocol.Pop3s;
			return SecProtocol.Invalid;
		}
	}
	
	internal static partial class KeysAuthenticationType {
		public static SecAuthenticationType ToSecAuthenticationType (IntPtr handle)
		{
			if (handle == IntPtr.Zero)
				return SecAuthenticationType.Invalid;
			if (CFType.Equal (handle, NTLM))
				return SecAuthenticationType.Ntlm;
			if (CFType.Equal (handle, MSN))
				return SecAuthenticationType.Msn;
			if (CFType.Equal (handle, DPA))
				return SecAuthenticationType.Dpa;
			if (CFType.Equal (handle, RPA))
				return SecAuthenticationType.Rpa;
			if (CFType.Equal (handle, HTTPBasic))
				return SecAuthenticationType.HttpBasic;
			if (CFType.Equal (handle, HTTPDigest))
				return SecAuthenticationType.HttpDigest;
			if (CFType.Equal (handle, HTMLForm))
				return SecAuthenticationType.HtmlForm;
			if (CFType.Equal (handle, Default))
				return SecAuthenticationType.Default;
			return SecAuthenticationType.Invalid;
		}

		public static IntPtr FromSecAuthenticationType (SecAuthenticationType type)
		{
			switch (type){
			case SecAuthenticationType.Ntlm:
				return NTLM;
			case SecAuthenticationType.Msn:
				return MSN;
			case SecAuthenticationType.Dpa:
				return DPA;
			case SecAuthenticationType.Rpa:
				return RPA;
			case SecAuthenticationType.HttpBasic:
				return HTTPBasic;
			case SecAuthenticationType.HttpDigest:
				return HTTPDigest;
			case SecAuthenticationType.HtmlForm:
				return HTMLForm;
			case SecAuthenticationType.Default:
				return Default;
			default:
				throw new ArgumentException ("type");
			}
		}
	}
	
	public class SecurityException : Exception {
		static string ToMessage (SecStatusCode code)
		{
			
			switch (code){
			case SecStatusCode.Success: 
			case SecStatusCode.Unimplemented: 
			case SecStatusCode.Param:
			case SecStatusCode.Allocate:
			case SecStatusCode.NotAvailable:
			case SecStatusCode.DuplicateItem:
			case SecStatusCode.ItemNotFound:
			case SecStatusCode.InteractionNotAllowed:
			case SecStatusCode.Decode:
				return code.ToString ();
			}
			return String.Format ("Unknown error: 0x{0:x}", code);
		}
		
		public SecurityException (SecStatusCode code) : base (ToMessage (code))
		{
		}
	}
}
