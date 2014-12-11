UrlRewrite 1.5
==============

While there are many Redirect / Rewrite modules available in the Sitecore Marketplace, I created this one to address the fact that most of them cannot create redirects that are Site specific.

In addition to adding Site specific redirect capabilities, I have modeled the data structure to closely match the functionality of Microsoft's IIS Url Rewrite plugin.


Changelog
=========

Version 1.5
* Added support for Custom Response
* Added support for Abort
* Added Bootstrap UI for testing the redirects and visually seeing how the rules are processed.
* Fixed UrlRewriteHandler so that it calls the StaticFileHandler if it no rules are matched. 

Version 1.1
* Major refactor to support instantiation of the UrlRewriter without needing HttpContext or a web site.  
* Supports Unit Tests.

Version 1.0
* Initial Release
