This is documentation
=====================

This branch is for converting the old <http://docs.structuremap.net> site from raw HTML into Markdown. Some 
people have put a large part of their lives toward writing documentation for StructureMap - mainly Jeremy
D. Miller.

Originally this site was written by hand in HTML. It's a huge pain to write and no one wants to read raw
HTML. Since no one wants to read raw HTML, no one wants to contribute back. Jeremy was an interesting 
writer who made documentation less painless, but once he stopped writing no one picked up where he left off.

Why Markdown?
-------------

[Markdown][1] is a wiki-style markup language that lets you write text in a text editor that looks like how
you would format regular plain text. A markdown engine then converts all this into semantic HTML. As such,
it's really easy to *read* the source markup. The hope is that, since it's easier to read, it's also easier
for anyone to contribute back. It's just plain text, so you should be able to use Github's inline edit
feature to edit documentation on the website.

What is Jekyll?
---------------

While markdown is great for writing documentation, we still need styling to our site. Jekyll is a static site
generator that is built into Github. It lets us setup layout pages to template our pages like ASP.NET MVC, 
except that all content is purely static HTML pages (there's nothing dynamic about documentation). Since it's
all static content, Github can serve it up fast and Google can index it well. 

Jekyll generates the HTML when you `git push` to Github. All the site's HTML is stored in the `_site/`, which
is ignored by our `.gitignore` file. That means that we don't check in the final HTML, we let Github generate
it as often as it needs to.

Jekyll is actually intended to be a blogging engine. When you're writing documentation it'll feel a little 
weird that you're doing things like prefixing the file name with a date or putting the documentation in the 
`_posts/` folder. It's a small hack that lets us make contributions extremely accessible.

Getting started
===============

These are the steps that you should follow to get started contributing to the *new* site and
converting the old HTML to markdown.

1. Fork structuremap using Github's Fork button
1. clone the repository: `git clone https://github.com/<your-username>/structuremap.git`
1. Switch to the `jekyll` branch: `git checkout jekyll` (you might have to run `git fetch origin jekyll`
2. *optional* Convert a file from the old site: `ruby html2md.rb old-site old-site/<name-of-file>.htm`. This
	writes a file called `_posts/<todays-date>-<name-of-file>.markdown` that's an 80% perfect conversion of
	the HTML. You should open it up in a text editor and fix any anomolies
3. Run Jekyll as a server: `jekyll --pygments --server`
4. Direct your browser to `http://localhost:4000/<name-of-file>/` to see how well it actually converted.
5. `git add -A` to stage all changes.
6. `git commit` with a great commit message
7. `git push` - this pushes to your fork of structuremap
8. Use github's website to send a pull request. I'll accept it soon

Prerequisites
-------------

You'll probably want to see what everything looks like locally before committing, so here are the steps to
setup your environment.

1. Install Ruby (I use version 1.9.3)
2. Run `gem install jekyll nokogiri rails` to install dependencies
3. Install Python
4. run `easy_install Pygments`

Pygments is the syntax highlighting engine that Jekyll uses to highlight our code samples. Since we have a
lot of code samples, this is an important part of your environment.

Rails isn't an important part of the environment. I use a single method out of ActionViews to wrap text in
my conversion script. If you're not using the conversion script, this isn't important.

Nokogiri is for the convertion script also. It's the most amazing HTML parser ever! It lets you navigate
the DOM using CSS or XPATH and pull `inner_text` or `inner_html` from nodes. It's indispensible to the 
conversion script. I really wish .NET had something like this.


 [1]: http://daringfireball.net/projects/markdown/syntax
