using System.Collections;
using System.ComponentModel;

namespace System.Configuration.Install
{
	/// <summary>Contains a collection of installers to be run during an installation.</summary>
	public class InstallerCollection : CollectionBase
	{
		private Installer owner;

		/// <summary>Gets or sets an installer at the specified index.</summary>
		/// <returns>An <see cref="T:System.Configuration.Install.Installer" /> that represents the installer at the specified index.</returns>
		/// <param name="index">The zero-based index of the installer to get or set. </param>
		public Installer this[int index]
		{
			get
			{
				return (Installer)base.List[index];
			}
			set
			{
				base.List[index] = value;
			}
		}

		internal InstallerCollection(Installer owner)
		{
			this.owner = owner;
		}

		/// <summary>Adds the specified installer to this collection of installers.</summary>
		/// <returns>The zero-based index of the added installer.</returns>
		/// <param name="value">An <see cref="T:System.Configuration.Install.Installer" /> that represents the installer to add to the collection. </param>
		public int Add(Installer value)
		{
			return base.List.Add(value);
		}

		/// <summary>Adds the specified collection of installers to this collection.</summary>
		/// <param name="value">An <see cref="T:System.Configuration.Install.InstallerCollection" /> that represents the installers to add to this collection. </param>
		public void AddRange(InstallerCollection value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			int count = value.Count;
			for (int i = 0; i < count; i++)
			{
				Add(value[i]);
			}
		}

		/// <summary>Adds the specified array of installers to this collection.</summary>
		/// <param name="value">An array of type <see cref="T:System.Configuration.Install.Installer" /> that represents the installers to add to this collection. </param>
		public void AddRange(Installer[] value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			for (int i = 0; i < value.Length; i++)
			{
				Add(value[i]);
			}
		}

		/// <summary>Determines whether the specified installer is included in collection.</summary>
		/// <returns>true if the specified installer is in this collection; otherwise, false.</returns>
		/// <param name="value">An <see cref="T:System.Configuration.Install.Installer" /> that represents the installer to look for. </param>
		public bool Contains(Installer value)
		{
			return base.List.Contains(value);
		}

		/// <summary>Copies the items from the collection to an array, begining at the specified index.</summary>
		/// <param name="array">The array to copy to. </param>
		/// <param name="index">The index of the array at which to paste the collection. </param>
		public void CopyTo(Installer[] array, int index)
		{
			base.List.CopyTo(array, index);
		}

		/// <summary>Determines the index of a specified installer in the collection.</summary>
		/// <returns>The zero-based index of the installer in the collection.</returns>
		/// <param name="value">The <see cref="T:System.Configuration.Install.Installer" /> to locate in the collection. </param>
		public int IndexOf(Installer value)
		{
			return base.List.IndexOf(value);
		}

		/// <summary>Inserts the specified installer into the collection at the specified index.</summary>
		/// <param name="index">The zero-based index at which to insert the installer. </param>
		/// <param name="value">The <see cref="T:System.Configuration.Install.Installer" /> to insert. </param>
		public void Insert(int index, Installer value)
		{
			base.List.Insert(index, value);
		}

		/// <summary>Removes the specified <see cref="T:System.Configuration.Install.Installer" /> from the collection.</summary>
		/// <param name="value">An <see cref="T:System.Configuration.Install.Installer" /> that represents the installer to remove. </param>
		public void Remove(Installer value)
		{
			base.List.Remove(value);
		}

		/// <summary>Performs additional custom processes before a new installer is inserted into the collection.</summary>
		/// <param name="index">The zero-based index at which to insert <paramref name="value" />.</param>
		/// <param name="value">The new value of the installer at <paramref name="index" />.</param>
		protected override void OnInsert(int index, object value)
		{
			if (value == owner)
			{
				throw new ArgumentException(Res.GetString("CantAddSelf"));
			}
			bool traceVerbose = System.ComponentModel.CompModSwitches.InstallerDesign.TraceVerbose;
			((Installer)value).parent = owner;
		}

		/// <summary>Performs additional custom processes before an installer is removed from the collection.</summary>
		/// <param name="index">The zero-based index at which <paramref name="value" /> can be found.</param>
		/// <param name="value">The installer to be removed from <paramref name="index" />. </param>
		protected override void OnRemove(int index, object value)
		{
			bool traceVerbose = System.ComponentModel.CompModSwitches.InstallerDesign.TraceVerbose;
			((Installer)value).parent = null;
		}

		/// <summary>Performs additional custom processes before an existing installer is set to a new value.</summary>
		/// <param name="index">The zero-based index at which to replace <paramref name="oldValue" />.</param>
		/// <param name="oldValue">The value to replace with <paramref name="newValue." /></param>
		/// <param name="newValue">The new value of the installer at <paramref name="index" />.</param>
		protected override void OnSet(int index, object oldValue, object newValue)
		{
			if (newValue == owner)
			{
				throw new ArgumentException(Res.GetString("CantAddSelf"));
			}
			bool traceVerbose = System.ComponentModel.CompModSwitches.InstallerDesign.TraceVerbose;
			((Installer)oldValue).parent = null;
			((Installer)newValue).parent = owner;
		}
	}
}
