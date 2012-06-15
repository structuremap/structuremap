# Script to convert hand-written HTML into Markdown documentation
#
# install dependencies with
# 	gem install nokogiri rails
#

require 'nokogiri'
require 'action_view'

class Processor
	include ActionView::Helpers::TextHelper

	def initialize(file) 
		@file = file
	end

	def process_body(body)
		body.children.each do |tag|
			if tag.name == 'p' then
				write_p tag
			elsif tag.name == 'h2' then
				write_h2 tag
			elsif tag.name == 'h4' then
				write_bold tag
			elsif tag.name == 'div' then
				if is_code tag then
					write_code tag
				end
			end
		end
	end

	def is_code tag
		if tag.has_attribute? 'class' and tag.attributes['class'].value == 'code-sample' then
			true
		elsif tag.has_attribute? 'style' and /.*font-family: Courier New;.*/ =~ tag.attributes['style'].value then
			true
		else
			false
		end
	end

	def write_p(tag)
		@file.puts ''
		@file.puts clean(tag.content)
		yield if block_given?
		@file.puts ''
	end

	def clean(text)
		text.gsub! /<\w*>/, '`\&`'
		wrap text
	end

	def write_h2(tag)
		write_p tag do
			@file.puts '================================='
		end
	end

	def write_bold tag
		@file.puts "**#{tag.inner_text}**"
	end

	def write_code(tag)
		@file.puts ''
		@file.puts '{% highlight csharp %}'

		tag.css('p,pre').each { |thing|
			@file.puts thing.inner_text
		}

		@file.puts '{% endhighlight %}'
		@file.puts ''
	end

	def create_newlines(tag)
		tag.children.each do |child|
			if child.name == 'p' then
				puts unwrap(child.inner_text)
				"#{unwrap(child.inner_text.to_s)}\r\n"
			else
				unwrap(child.inner_text.to_s)
			end
		end
	end

	def unwrap(text)
		unwrapped = text.gsub /(\r?\n|\t|(?=< ) *)/, ''
	end

	def wrap(text)
		word_wrap unwrap(text)
	end
end

@document = Nokogiri::HTML(File.open(ARGV[0]))

outfile = File.basename(ARGV[0], '.*')
File.open "#{outfile}.markdown", 'w' do |file|
	file.puts '---'
	file.puts "title: #{@document.css('title').first.content}"
	file.puts 'layout: default'
	file.puts '---'
	p = Processor.new file
	p.process_body @document.css('body').first
end
