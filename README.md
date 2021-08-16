# CrawlAPI
Fanmade Crawl modding API

Easy Install:

go to Releases on the right of this page, download the zip, then extract its contents into your crawl's game folder.



Compile Instructions(if you want to make changes to the api, not develop for the api):

first, compile and patch using CrawlPrePatcher, then dump the assembly using the config file and add a reference to the patched and dumped assembly csharp to your crawlapi.
then you should be able to build crawlapi, make sure to set all assembly references to false on "copy local" except for MMHOOK_assembly_csharp, which should be set to true.
lemme know if you get stuck at `Bread Man#8916` on discord
