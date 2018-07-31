using System.Runtime.Serialization;

namespace System.Configuration.Install
{
	/// <summary>The exception that is thrown when an error occurs during the commit, rollback, or uninstall phase of an installation.</summary>
	[Serializable]
	public class InstallException : SystemException
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.Install.InstallException" /> class.</summary>
		public InstallException()
		{
			HResult = -2146232057;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.Install.InstallException" /> class, and specifies the message to display to the user.</summary>
		/// <param name="message">The message to display to the user. </param>
		public InstallException(string message)
			: base(message)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.Install.InstallException" /> class, and specifies the message to display to the user, and a reference to the inner exception that is the cause of this exception.</summary>
		/// <param name="message">The message to display to the user. </param>
		/// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter is not null, the current exception is raised in a catch block that handles the inner exception. </param>
		public InstallException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.Install.InstallException" /> class with serialized data.</summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown. </param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination. </param>
		protected InstallException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
