Url Rewrite
===========

While there are many Redirect / Rewrite modules available in the Sitecore Marketplace, I created this one to address the fact that most of them cannot create redirects that are Site specific.

In addition to adding Site specific redirect capabilities, I have modeled the data structure to closely match the functionality of Microsoft's IIS Url Rewrite plugin.

*** This module is not supported if you are using xDB Cloud Edition ***
Sitecore Support: "Creation of custom segments is not supported by the Experience Analytics in the xDB Cloud service. This should be fixed in a future service update." https://kb.sitecore.net/articles/966080


Changelog
=========

Version 1.9
* Added support for Sitecore 9

Varsion 1.8.2 - https://github.com/iamandycohen/UrlRewrite/releases/tag/v1.8.2
* Added support for Sitecore 8.2

Varsion 1.8.1.3 - https://github.com/iamandycohen/UrlRewrite/releases/tag/v1.8.1.3
* Added support for Sitecore 8.1 Update 2 & Update 3
* Fixed sort order thanks to Dan Sinclair
* Fixed RulesEngine.GetRedirectFolderItems so that it only returns Redirect Folder templates

Version 1.8 - https://github.com/iamandycohen/UrlRewrite/releases/tag/v1.8
* Added reporting via analytics
* added Url Rewrite to Experience Analytics Dashboard
* Added prefetching of UrlRewriter item
* Moved all templates from User Defined folder to System Modules
* Removed unused references
* Fixed logic for allowing multiple rules to be processed during a request when a Redirect field "Stop Processing of Subsequent Rules" is set to false
* Package now installs to master and web

Version 1.7.2 - https://github.com/iamandycohen/UrlRewrite/releases/tag/v1.7.2
* Fixed potential for a redirect loop if you have a database problem, you are using the html extension on the handler, and you have a custom error page that is pointing to an html page
* Changed initialization of rules into an initialization pipeline
* Removed reporting
* Changed error handling of 404's on a static file redirect
* Fixed rebuilding of cache on save, publish or delete - no longer need to restart IIS to refresh the rules.
* Added missing update to Default.aspx for UrlRewrite tester

Version 1.7.1 - https://github.com/iamandycohen/UrlRewrite/releases/tag/v1.7.1
* Fixed missing reporting item in the web database that prevented publishing from working after installing the module.

Version 1.7 - https://github.com/iamandycohen/UrlRewrite/releases/tag/v1.7
* Added outbound rules
* Added ability to set response headers
* Added inbound rule rewrite action
* General cleanup
* Added Sample Rewrites
* Added hit count tracking

Version 1.6 - https://github.com/iamandycohen/UrlRewrite/releases/tag/v1.6
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
