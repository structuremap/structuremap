COMPILE_TARGET = ENV['config'].nil? ? "debug" : ENV['config']
RESULTS_DIR = "results"

task :default => [:test]

desc "Prepares the working directory for a new build"
task :clean do
	#TODO: do any other tasks required to clean/prepare the working directory
	FileUtils.rm_rf RESULTS_DIR

end

task :compile => [:clean] do
	sh "C:/Windows/Microsoft.NET/Framework/v4.0.30319/msbuild.exe src/StructureMap.sln   /property:Configuration=#{COMPILE_TARGET} /v:m /t:rebuild /nr:False /maxcpucount:2"
end

task :test => [:compile] do
	Dir.mkdir RESULTS_DIR

	sh "packages/Fixie/lib/net45/Fixie.Console.exe src/StructureMap.Testing/bin/#{COMPILE_TARGET}/StructureMap.Testing.dll --NUnitXml results/TestResult.xml"
end