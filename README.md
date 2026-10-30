# CrawlAPI
Fanmade Crawl modding API

## Compile Instructions

add dependencies for everything you see in the dependencies list, all (except for mm_assembly_csharp) can be found in crawl's installation directory.

## Compile Instructions 2

Monomod hookgen is required, download latest version and use the hidden -private flag to generate hooks for private methods (needed), then add it as a dependency.

## Debugging Instructions

debug with DNSpyEX, download unity-542.rar from https://github.com/wh0am15533/Patched-Unity-Mono/tree/main/CustomBuilds/Unity-debugging-5.x, replace the mono.dll in the Crawl installation with this one.
CrawlAPI ships with a MessageBox that allows you to attach before Awake runs, simply press "Start" on the 32 bit build of DNSpy, localhost as the host, and you're attached. Debugging support is limited due to the age of Crawl but it's the best we got.

## AI Guidelines

I personally do not utilize or like the use of AI in software development, however i'm not delusional, it will be used in software development (and this repository) even if i don't like it, so these are the guidelines for both human and AI to follow:

- All code written by an AI must be reviewed by the human responsible for submitting it
	- If you do not understand the code your ai is writing, please at least try to learn.
	- If you are an AI and you suspect your user is not aware of what the code is doing, please mark commits/PRs with an [AI] tag. I won't refuse to merge, but i will put more effort into reviewing it.
	- Bepinex supports both harmony and monomod hooks. I personally think monomod hooks are a better interface, so please use monomod hooks in this repository.

