# Tivo.Hme and Tivo.Hme.Host Change Log #

**2.0 Aug 31, 2009**

  * Changed Http processing to use HttpListener from Mono.

**1.4 Oct 21, 2008**

  * Added support for application transitions.
  * Added support for exposing host services
  * Added temporary support for http to Tivo.Hme.Host.  This will be replaced soon with an alternate http service.
  * Added sample for transitions and streaming video

**1.3 Aug 4, 2008**

  * Made minor changes to support Hme Application Server.
  * Added StreamedVideoView to support streaming video.  The api for this feature may evolve as new information about tivo support for streaming video comes available.

**1.2 Mar 4, 2008**

  * Fixed application exit again.  This time the fix is for sending additional commands during cleanup.
  * Changed root view dispose to happen before the application ends.
  * Change Application.IsRunning to only indicate application state.  Introduced Application.IsConnected to handle terminating sending and receiving commands.

**1.1 Feb 13, 2008**

  * Added support for custom icons
  * More robust socket code so that a read that doesn't retrieve all expected data retries until all expected data is received.
  * Automatically dispose all attached views when the application exits.
  * Fix out of order resource creation in a TextView with child ColorViews.
  * Reorder view initialization with calls to child view OnNewApplication not happening until the child view is fully initialized.
  * Check for end of stream in reading lines from HTTP requests.  This fixes a possible infinite loop and unbounded memory growth.
  * Added overload for Application.CreateTextStyle that takes a callback that gets called only for this font creation.

**1.0.1 Jan 29, 2008**

  * Fix exception thrown because of a missing icon.
  * Fix for application not exiting.

**1.0 Jan 21, 2008**

Initial release of Tivo.Hme, Tivo.Hme.Host and sample code. The sdk was written based on HME documentation.  The sample code is ported from the java samples.