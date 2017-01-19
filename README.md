WavTexture
==========

A Unity example showing how to bake a waveform of an audio clip into a texture and use it in a shader.

![gif](http://68.media.tumblr.com/30f650ff39f960963c7b8b0e1a090570/tumblr_ok12382h421qio469o1_500.gif)

Why should I bake a waveform into a texture?
--------------------------------------------

- Pros: You can  render a waveform with almost zero cost on the CPU side. It never waste heap/GC emmory.
- Cons: You have to bake a waveform at build time. It's not very useful for interactive software.

In other words, this technique is only useful when you're creating something not interactive,
and you prefer utilizing GPU rather than sacrificing the precious CPU time.
