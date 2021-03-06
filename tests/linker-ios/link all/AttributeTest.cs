//
// Link All attribute-related Tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013-2016 Xamarin Inc. All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;

#if XAMCORE_2_0
using Foundation;
#else
using MonoTouch.Foundation;
#endif

using NUnit.Framework;

[assembly: Debuggable (DebuggableAttribute.DebuggingModes.Default)]

[assembly: LinkAll.Attributes.CustomAttributeArray (typeof (LinkAll.Attributes.CustomTypeA))]
[assembly: LinkAll.Attributes.CustomAttribute (typeof (LinkAll.Attributes.CustomType))]

namespace LinkAll.Attributes {

	// the linker removes those attributes on release builds but must keep them in debug builds
	[DebuggerDisplay ("value")]
	[DebuggerNonUserCode]
	[DebuggerStepThrough]
	[DebuggerTypeProxy ("")]
	[DebuggerVisualizer ("")]
	class FullyDecorated {

		[DebuggerHidden]
		[DebuggerStepperBoundary]
		public FullyDecorated ()
		{
		}

		[DebuggerBrowsable (DebuggerBrowsableState.Collapsed)]
		public int Property { get; set; }
	}

	public class CustomAttributeArray : Attribute {
		readonly Type[] _types;

		public CustomAttributeArray (params Type[] types)
		{
			_types = types;
		}
	}

	public class CustomTypeA {
	}

	public class CustomAttribute : Attribute {
		readonly Type _type;

		public CustomAttribute (Type type)
		{
			_type = type;
		}
	}

	public class CustomType {
	}

	[FileIOPermission (SecurityAction.LinkDemand, AllLocalFiles = FileIOPermissionAccess.AllAccess)]
	public class SecurityDeclarationDecoratedUserCode {

		[FileIOPermission (SecurityAction.Assert, AllLocalFiles = FileIOPermissionAccess.NoAccess)]
		static public bool Check ()
		{
			return true;
		}
	}

	[TestFixture]
	// we want the tests to be available because we use the linker
	[Preserve (AllMembers = true)]
	public class AttributeTest {
		
		[Test]
		public void DebugAssemblyAttributes ()
		{
			bool result = false;
			foreach (object ca in typeof(FullyDecorated).Assembly.GetCustomAttributes (false)) {
				if (ca is DebuggableAttribute)
					result = true;
			}
#if DEBUG
			Assert.True (result, "DebuggableAttribute");
#else
			Assert.False (result, "DebuggableAttribute");
#endif
		}

		[Test]
		public void DebugTypeAttributes ()
		{
			var ca = typeof(FullyDecorated).GetCustomAttributes (false);
#if DEBUG
			Assert.That (ca.Length, Is.EqualTo (5), "Debug attributes in debug mode");
#else
			Assert.That (ca.Length, Is.EqualTo (0), "no debug attribute in release mode");
#endif
		}

		[Test]
		public void DebugConstructorAttributes ()
		{
			var ca = typeof(FullyDecorated).GetConstructor (Type.EmptyTypes).GetCustomAttributes (false);
#if DEBUG
			Assert.That (ca.Length, Is.EqualTo (2), "Debug attributes in debug mode");
#else
			Assert.That (ca.Length, Is.EqualTo (0), "No debug attribute in release mode");
#endif
		}

		[Test]
		public void DebugPropertyAttributes ()
		{
			// Ensure the linker won't remove them. Note: we do not want to use [Preserve] 
			// since it wculd change how the linker process them (and the attributes)
			var c = new FullyDecorated ();
			c.Property = 1;

			bool result = false;
			foreach (object ca in typeof (FullyDecorated).GetProperty ("Property").GetCustomAttributes (false)) {
				if (ca is DebuggerBrowsableAttribute)
					result = true;
			}
#if DEBUG
			Assert.True (result, "DebuggerBrowsable");
#else
			Assert.False (result, "DebuggerBrowsable");
#endif
		}

		[Test]
		public void DebuggerTypeProxy_24203 ()
		{
			var d = new Dictionary<string,int> () { { "key", 0 } };
			Assert.NotNull (d); // just to be 100% sure it won't be linked away (with the attribute we'll be looking for)
			var proxy = Type.GetType ("System.Collections.Generic.Mscorlib_DictionaryDebugView`2, mscorlib");
#if DEBUG
			Assert.NotNull (proxy, "proxy");
			// having the type is nice, but it must not be empty to be useful
			Assert.That (proxy.GetConstructors ().Length, Is.GreaterThan (0), "constructors");
			Assert.That (proxy.GetProperties ().Length, Is.GreaterThan (0), "properties");
#else
			Assert.Null (proxy, "proxy");
#endif
		}

		[Test]
		public void CustomAttributesWithTypes ()
		{
			var assembly = GetType ().Assembly;
			var ta = assembly.GetCustomAttributes<CustomAttributeArray> ();
			Assert.That (ta.Count (), Is.EqualTo (1), "Type[]");
			Assert.NotNull (Type.GetType ("LinkAll.Attributes.CustomTypeA"), "CustomTypeA");

			var t = assembly.GetCustomAttributes<CustomAttribute>();
			Assert.That (t.Count (), Is.EqualTo (1), "Type");
			Assert.NotNull (Type.GetType ("LinkAll.Attributes.CustomType"), "CustomType");
		}

		[Test]
		public void SecurityDeclaration ()
		{
			// note: security declarations != custom attributes
			// we ensure that we can create the type / call the code
			Assert.True (SecurityDeclarationDecoratedUserCode.Check (), "call");
			// we ensure that both the permission and the flag are NOT part of the final/linked binary (link all removes security declarations)
			Assert.Null (Type.GetType ("System.Security.Permissions.FileIOPermissionAttribute, mscorlib"), "FileIOPermissionAttribute");
			Assert.Null (Type.GetType ("System.Security.Permissions.FileIOPermissionAccess, mscorlib"), "FileIOPermissionAccess");
		}
	}
}