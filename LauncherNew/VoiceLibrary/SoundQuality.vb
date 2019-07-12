Imports POpusCodec
Imports POpusCodec.Enums

Module SoundQuality
    Public encoder_kilobytes = 8                            'KB per second for sound encoder
    Public encoder_bitrate = encoder_kilobytes * (1024 * 8) 'Do not touch this. Automatically calculated based on KB.
    Public encoder_delay = Delay.Delay60ms                  'This value can be tweaked if there is stuttering. 'Public encoder_delay = Delay.Delay60ms          
    Public encoder_maxband = Bandwidth.SuperWideband        'Superwideband is recommended. Using fullband will include unused frequencies.
    Public encoder_type = OpusApplicationType.Audio         'Set to audio for better quality.
    Public encoder_raw = False                              'Raw PCM sound will be encoded if true (not recommended).

    Public sound_output_buffer = (encoder_delay / 2) * 6    'This value should be at least 3 times encode_delay.
    Public sound_output_dx = False                          'Enables DirectSound Output. May introduce increased latency or problems (experimental).
    Public sound_input_buffer = encoder_delay / 2           'Do not touch this. It is automatically set based on encoder_delay.

    'Changing the below is not recommended
    Public sound_bps = 16                                   'Bits per sample
    Public sound_channels = 1                               'Stereo or Mono
    Public sound_samplerate = SamplingRate.Sampling24000    'Sample rate HZ
    Public sound_mixerchannels = 32                         'Max channels that can be mixed on VoiceClient (set for future proofing, there is no performance penalty)
End Module