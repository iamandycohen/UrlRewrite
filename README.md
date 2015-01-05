UrlRewrite
==========

While there are many Redirect / Rewrite modules available in the Sitecore Marketplace, I created this one to address the fact that most of them cannot create redirects that are Site specific.

In addition to adding Site specific redirect capabilities, I have modeled the data structure to closely match the functionality of Microsoft's IIS Url Rewrite plugin.


Changelog
=========

Version 1.7
* Added outbound rules
* Added ability to set response headers
* Added inbound rule rewrite action
* General cleanup
* Added Sample Rewrites

Version 1.6
* Added information about conditions to the Url Rewrite application in the Sitecore Admin UI
* Fixed UrlRewriteHandler displaying blank page if the StaticMapHandler didn't find the file. Allowed exceptions to be bubbled up.
* Added a new type of rediret action - Item Query Redirect which allows you to use a Sitecore Query to find an item.  Ability to utilize back references from conditions.

Version 1.5
* Added support for Custom Response
* Added support for Abort
* Added Bootstrap UI for testing the redirects and visually seeing how the rules are processed.
* Fixed UrlRewriteHandler so that it calls the StaticFileHandler if it no rules are matched. 
* Added drop down list so you can select your site instead of having to type a regular expression to match the site on.  NOTE *** All Redirect Folder's are currently reset so you have to reset the Site Name Restriction field.
* Added the ability to have sub folders in a top level Redirect Folder
* removed the top level Redirect Folder from the package so that rules don't get overwritten when you reinstall the package
* Updated how back references work for Rules and Conditions
* Fixed security on Redirect Workflow

Version 1.1
* Major refactor to support instantiation of the UrlRewriter without needing HttpContext or a web site.  
* Supports Unit Tests.

Version 1.0
* Initial Release
