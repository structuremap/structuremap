require 'json'

COMPILE_TARGET = ENV['config'].nil? ? "debug" : ENV['config']
RESULTS_DIR = "results"
BUILD_VERSION = '4.5.2'

DOC_LOCATION = ENV['docpath'].nil? ? "z:/code/structuremap.github.com" : ENV['docpath']

tc_build_number = ENV["BUILD_NUMBER"]
build_revision = tc_build_number || Time.new.strftime('5%H%M')
build_number = "#{BUILD_VERSION}.#{build_revision}"
BUILD_NUMBER = build_number 

task :ci => [:default, :pack]

task :default => [:test]

desc "Prepares the working directory for a new build"
task :clean do
	#TODO: do any other tasks required to clean/prepare the working directory
	FileUtils.rm_rf RESULTS_DIR
	FileUtils.rm_rf 'artifacts'

end

desc "Update the version information for the build"
task :version do
  asm_version = BUILD_VERSION
  
  begin
    commit = `git log -1 --pretty=format:%H`
  rescue
    commit = "git unavailable"
  end
  puts "##teamcity[buildNumber '#{build_number}']" unless tc_build_number.nil?
  puts "Version: #{build_number}" if tc_build_number.nil?
  
  options = {
	:description => 'IoC Container for .Net',
	:product_name => 'StructureMap',
	:copyright => 'Copyright 2004-2017 Jeremy D. Miller, Joshua Flanagan, Frank Quednau, Tim Kellogg, et al. All rights reserved.',
	:trademark => commit,
	:version => asm_version,
	:file_version => BUILD_VERSION,
	:informational_version => asm_version
	
  }
  
  puts "Writing src/CommonAssemblyInfo.cs..."
	File.open('src/CommonAssemblyInfo.cs', 'w') do |file|
		file.write "using System.Reflection;\n"
		file.write "using System.Runtime.InteropServices;\n"
		file.write "[assembly: AssemblyDescription(\"#{options[:description]}\")]\n"
		file.write "[assembly: AssemblyProduct(\"#{options[:product_name]}\")]\n"
		file.write "[assembly: AssemblyCopyright(\"#{options[:copyright]}\")]\n"
		file.write "[assembly: AssemblyTrademark(\"#{options[:trademark]}\")]\n"
		file.write "[assembly: AssemblyVersion(\"#{options[:version]}\")]\n"
		file.write "[assembly: AssemblyFileVersion(\"#{options[:file_version]}\")]\n"
		file.write "[assembly: AssemblyInformationalVersion(\"#{options[:informational_version]}\")]\n"
	end
	

end

desc 'Compile the code'
task :compile => [:clean, :version] do
	sh "dotnet restore src"
	sh "dotnet build src/StructureMap"
	sh "dotnet build src/StructureMap.AutoFactory"
	sh "dotnet build src/StructureMap.DynamicInterception"
	sh "dotnet build src/StructureMap.AutoFactory.Testing"
	sh "dotnet build src/StructureMap.DynamicInterception.Testing"
end

desc 'Run the unit tests'
task :test => [:compile] do
	Dir.mkdir RESULTS_DIR

	sh "dotnet test src/StructureMap.Testing/StructureMap.Testing.csproj"
	sh "dotnet test src/StructureMap.AutoFactory.Testing/StructureMap.AutoFactory.Testing.csproj"
	sh "dotnet test src/StructureMap.DynamicInterception.Testing/StructureMap.DynamicInterception.Testing.csproj"
end

desc 'Build Nuspec packages'
task :pack => [:compile] do
	sh "dotnet pack src/StructureMap -o artifacts --configuration Release"
	sh "dotnet pack src/StructureMap.AutoFactory -o artifacts --configuration Release"
	sh "dotnet pack src/StructureMap.DynamicInterception -o artifacts --configuration Release"
end

desc "Launches VS to the StructureMap solution file"
task :sln do
	sh "start src/StructureMap.sln"
end

"Launches the documentation project in editable mode"
task :docs do
	sh "dotnet restore"
	sh "dotnet stdocs run -v #{BUILD_VERSION}"
end

"Exports the documentation to structuremap.github.io - requires Git access to that repo though!"
task :publish do
	

	if !Dir.exists? 'doc-target' 
		Dir.mkdir 'doc-target'
		sh "git clone https://github.com/structuremap/structuremap.github.com.git doc-target"
	else
		Dir.chdir "doc-target" do
			sh "git checkout --force"
			sh "git clean -xfd"
			sh "git pull origin master"
		end
	end
	
	sh "dotnet restore"
	sh "dotnet stdocs export doc-target Website --version #{BUILD_VERSION}"
	
	Dir.chdir "doc-target" do
		sh "git add --all"
		sh "git commit -a -m \"Documentation Update for #{BUILD_VERSION}\" --allow-empty"
		sh "git push origin master"
	end
	

	

end

def load_project_file(project)
  File.open(project) do |file|
    file_contents = File.read(file, :encoding => 'bom|utf-8')
    JSON.parse(file_contents)
  end
end
