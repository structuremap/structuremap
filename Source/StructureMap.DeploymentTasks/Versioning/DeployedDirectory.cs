using System.Collections;
using System.IO;
using System.Xml.Serialization;

namespace StructureMap.DeploymentTasks.Versioning
{
	public class DeployedDirectory
	{
		private string _name;
		private Hashtable _childDirectories;
		private Hashtable _files;
		private Hashtable _assemblies;

		public DeployedDirectory()
		{
			_childDirectories = new Hashtable();
			_files = new Hashtable();
			_assemblies = new Hashtable();
		}

		public DeployedDirectory(DirectoryInfo directory) : this()
		{
			_name = directory.Name;

//			foreach (DirectoryInfo childDirectory in directory.GetDirectories())
//			{
//				DeployedDirectory child = new DeployedDirectory(childDirectory);
//				AddChildDirectory(child);
//			}

			foreach (FileInfo fileInfo in directory.GetFiles())
			{
				DotNetAssembly assembly = DotNetAssembly.TryCreateAssembly(fileInfo);
				if (assembly != null)
				{
					this.AddAssembly(assembly);
				}
				else
				{
					DeployedFile deployedFile = DeployedFile.CreateFile(fileInfo);
					this.AddFile(deployedFile);
				}
			}
		}

		public void CheckDeployedVersions(DeployedDirectory deployedDirectory, IVersionReport report)
		{
			foreach (DeployedFile file in _files.Values)
			{
				file.CheckVersion(deployedDirectory, report);
			}

			foreach (DotNetAssembly dotNetAssembly in _assemblies.Values)
			{
				dotNetAssembly.CheckVersion(deployedDirectory, report);
			}
			{
				
			}
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

//		public DeployedDirectory[] ChildDeployedDirectories
//		{
//			get
//			{
//				DeployedDirectory[] children = new DeployedDirectory[_childDirectories.Count];
//				_childDirectories.Values.CopyTo(children, 0);
//
//				return children;
//			}
//			set
//			{
//				_childDirectories = new Hashtable();
//				foreach (DeployedDirectory directory in value)
//				{
//					AddChildDirectory(directory);
//				}
//			}
//		}
//
//		public void AddChildDirectory(DeployedDirectory deployedDirectory)
//		{
//			_childDirectories.Add(deployedDirectory.Name, deployedDirectory);
//		}
//
//		public DeployedDirectory FindChildDirectory(string directoryName)
//		{
//			return _childDirectories[directoryName] as DeployedDirectory;
//		}



		public DeployedFile[] DeployedFiles
		{
			get
			{
				DeployedFile[] children = new DeployedFile[_files.Count];
				_files.Values.CopyTo(children, 0);

				return children;
			}
			set
			{
				_files = new Hashtable();
				foreach (DeployedFile file in value)
				{
					AddFile(file);
				}
			}
		}

		public void AddFile(DeployedFile deployedFile)
		{
			_files.Add(deployedFile.FileName.ToUpper(), deployedFile);
		}

		public DeployedFile FindFile(string fileName)
		{
			return _files[fileName] as DeployedFile;
		}


		public DotNetAssembly[] Assemblies
		{
			get
			{
				DotNetAssembly[] assemblies = new DotNetAssembly[_assemblies.Count];
				_assemblies.Values.CopyTo(assemblies, 0);

				return assemblies;
			}
			set
			{
				_assemblies = new Hashtable();
				foreach (DotNetAssembly netAssembly in value)
				{
					AddAssembly(netAssembly);
				}
			}
		}

		public void AddAssembly(DotNetAssembly netAssembly)
		{
			_assemblies.Add(netAssembly.AssemblyName, netAssembly);
		}

		public DotNetAssembly FindAssembly(string assemblyName)
		{
			return _assemblies[assemblyName] as DotNetAssembly;
		}

		public void WriteToXml(string fileName)
		{
			XmlSerializer serializer = new XmlSerializer(this.GetType());
			using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
			{
				serializer.Serialize(stream, this);
			}
		}


		public static DeployedDirectory ReadFromXml(string fileName)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(DeployedDirectory));
			using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			{
				return (DeployedDirectory) serializer.Deserialize(stream);
			}
		}
	}
}
