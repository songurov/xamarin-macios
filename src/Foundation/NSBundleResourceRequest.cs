using System;
using System.Reflection;
using System.Collections;
using System.Runtime.InteropServices;

using XamCore.ObjCRuntime;

#if !MONOMAC

namespace XamCore.Foundation {
	public partial class NSBundleResourceRequest : NSObject {
#if XAMCORE_2_0
		static NSSet<NSString> MakeSetFromTags (string [] tags)
		{
			var x = new NSString [tags.Length];
			for (int i = 0; i < tags.Length; i++)
				x [i] = new NSString (tags [i]);
			return new NSSet<NSString> (x);
		}

		static NSSet<NSString> MakeSetFromTags (NSString [] tags)
		{
			return new NSSet<NSString> (tags);
		}
#else
		static NSSet MakeSetFromTags (string [] tags)
		{
			var x = new NSString [tags.Length];
			for (int i = 0; i < tags.Length; i++)
				x [i] = new NSString (tags [i]);
			return new NSSet (x);
		}

		static NSSet MakeSetFromTags (NSString [] tags)
		{
			return new NSSet (tags);
		}
#endif
		
		public NSBundleResourceRequest (params string [] tags) : this (MakeSetFromTags (tags)) {}
		public NSBundleResourceRequest (NSBundle bundle, params string [] tags) : this (MakeSetFromTags (tags), bundle) {}

		public NSBundleResourceRequest (params NSString [] tags) : this (MakeSetFromTags (tags)) {}
		public NSBundleResourceRequest (NSBundle bundle, params NSString [] tags) : this (MakeSetFromTags (tags), bundle) {}

	}
}

#endif
