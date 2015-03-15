.NET libraries for developing and hosting HME applications written in C#.  It uses [Mono.ZeroConf](http://www.mono-project.com/Mono.Zeroconf) for local network applications.  This in turn depends on [bonjour from apple](http://www.apple.com/support/downloads/bonjourforwindows.html).  Visual Studio 2008 projects are included in the source.

The HME library was built based on Tivo documentation for the protocol.  This library is not provided by or supported by Tivo.

Please contact me if you have any questions or feedback.

Josh Cooley -
[jbtivo@tuxinthebox.net](mailto:jbtivo@tuxinthebox.net)

## News/Updates ##

September 13, 2009

I've released a bug fix for the Tivo.Hmo library that allows content to be downloaded from a tivo.  Thanks to the mono team I was able to use some of their code to work around an issue with the cookies sent by a tivo.

August 31, 2009

This has been waiting in SVN for a while.  It incorporates all the changes mentioned below.

February 11, 2009

I've been working on the 2.0 release.  It is almost complete and now in checked in to the source tree.  The main feature for this release is a replacement of the host http services.  Mp3s and streaming videos have a better web server to provide these files thanks to the Mono project.  I've incorporated the web server portions into the Tivo.Hme.Host.dll but have added a dependency on Mono.Security.  This new dependency will be part of the package when a new release is made.

In addition to source changes, you may notice three new links in the featured wiki pages on the right.  These are entries into the api docs for Tivo.Hme.dll.  The documentation is a little thin right now, but will be updated as soon as I go through and add the necessary XML comments.

October 21, 2008

The new release is now out.  It's a minor update for the HME library.  This release includes a refresh of some samples as well as some hosting changes.  I provide an http service available to HME apps.  It's just a prototype and will definitely change in later releases.  The disk usage app does have the promised bug fix as well as the new HMO sdk.  There's no independent download of that yet.  I'm still trying to figure out what else is necessary for a useful HMO sdk.

October 14, 2008

I've been working on a sample for the video streaming as well as support and sample for the transport command.  I should have a release of the hme sdk, samples, and disk usage app soon.  The disk usage application has an overhauled hmo library that should fix a reported bug as well as be the beginning of an independently usable hmo library.  It's currently very limited, but is closer to what I want than the previous version.

August 4, 2008

An updated version of the library has been released that supports streaming video.  At the same time, I've released a preview of the hosting service for hme applications.  It runs as a windows service and requires manual configuration currently.  Please check the readme in the HmeApplicationServer directory for very basic installation instructions.  As stated earlier, I plan on having a configuration tool to help with administering this service.

June 5, 2008

I've been working on a new release for a while.  The main enhancement is a new service that can host hme applications.  The main work left is to build a configuration tool and an installer.  If your interested, you can download the source and check it out.