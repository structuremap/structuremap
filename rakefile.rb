require 'fuburake'


FubuRake::Solution.new do |sln|
	sln.compile = {
		:solutionfile => 'src/StructureMap.sln'
	}
				 
	sln.assembly_info = {
		:product_name => "StructureMap",
		:copyright => 'Copyright 2004-2014 Jeremy D. Miller, Joshua Flanagan, Frank Quednau, Tim Kellogg, et al. All rights reserved.'
	}
	
	#sln.compile_targets = ['Debug', 'Release', 'NET45WP8']
	
	sln.ripple_enabled = false
	sln.fubudocs_enabled = false
	
	#sln.ci_steps = ['compile:net45wp8']

end

