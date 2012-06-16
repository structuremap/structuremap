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
			elsif tag.name == 'h1' then
				write_h1 tag
			elsif tag.name == 'h2' then
				write_h2 tag
			elsif tag.name == 'h3' then
				write_header tag, 3
			elsif tag.name == 'h4' then
				write_header tag, 4
			elsif tag.name == 'h5' then
				write_header tag, 5
			elsif tag.name == 'h6' then
				write_header tag, 6
			elsif tag.name == 'div' or tag.name == 'code' then
				if is_code tag then
					write_code tag
				end
			elsif tag.name == 'ul' or tag.name == 'ol' then
				write_list tag
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
		tag = expand_links tag
		tag = recover_formatting tag
		tag = tag.content if tag.respond_to? 'content'
		return unless tag

		tag = codify_things_that_look_like_code tag

		@file.puts ''
		@file.puts clean(tag)
		yield if block_given?
		@file.puts ''
	end

	def recover_formatting(tag)
		return tag unless tag.respond_to? 'xpath'

		tag.xpath('//b').each do |b| 
			b.replace @document.create_text_node("**#{b.inner_text}**")
		end

		tag.xpath('//i').each do |i| 
			b.replace @document.create_text_node("*#{i.inner_text}*")
		end

		tag
	end

	def codify_things_that_look_like_code(text)
		# If it has an uppercase letter in the middle of the word, unless it's not code
		text.gsub /\w+[A-Z]\w*(<.*?>)?|\w+\(.*?\)/ do |match|
			if code_in_paragraph? match.to_s then
				"`#{match}`" 
			else
				match
			end
		end
	end

	def code_in_paragraph?(text)
		not_code = ['StructureMap', 'IoC', 'StructueMap', 'NHibernate']

		# checking that it's not special, and isn't all caps
		unless not_code.include? text or text.to_s =~ /^[A-Z]+$/ then
			true
		else
			false
		end
	end

	def expand_links(tag)
		if tag.respond_to? 'xpath' then
			tag.xpath('//a').each do |a|
				inner_text = unwrap a.inner_text
				text = "[#{inner_text}](#{a['href']})"
				a.replace @document.create_text_node(text)
			end
		end
		tag
	end

	def clean(text)
		# I was seeing some places where <T> was being falsely interpreted as HTML
		# this is a bit naive but it works. Fix it by hand if it's really GetInstance<T>
		text.gsub! /<\w*>/, '`\&`'
		wrap text
	end

	def write_h2(tag)
		write_p tag do
			@file.puts '================================='
		end
	end

	def write_h2(tag)
		write_p tag do
			@file.puts '---------------------------------'
		end
	end

	def write_header tag, size
		write_p "#{'#' * size} #{tag.inner_text}"
	end

	def write_code(tag)
		@file.puts ''
		@file.puts '{% highlight csharp %}'

		tag.css('p,pre').each do |thing|
			text = thing.inner_text
			text = unwrap text if thing.name == 'p'
			text = strip_insignificant_space(text)
			@file.puts text
		end

		@file.puts '{% endhighlight %}'
		@file.puts ''
	end

  def strip_insignificant_space(line)
    line.gsub /(^\s*\w.*?)\s\s+$/, '\1'
  end

	def write_list(tag)
		if tag.name == 'ul' then
			bullet = '*'
		else
			bullet = '1.'
		end

		tag.xpath('li').each do |li|
			@file.puts "#{bullet} #{wrap li.inner_text}"
		end
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
		unwrapped.gsub(/ +/, ' ')
	end

	def wrap(text)
		word_wrap unwrap(text)
	end

	def document=(doc)
		@document = doc
	end
end

@document = Nokogiri::HTML(File.open(ARGV[0]))

outfile = File.basename(ARGV[0], '.*')
File.open "_posts/#{Time.new.strftime('%Y-%m-%d')}-#{outfile}.markdown", 'w' do |file|
	file.puts '---'
	file.puts "title: #{@document.css('title').first.content}"
	file.puts 'layout: default'
	file.puts '---'
	p = Processor.new file
	p.document = @document
	p.process_body @document.css('body').first
end
