﻿using System;
using System.IO;

using PhotoSauce.MagicScaler.Interop;

namespace PhotoSauce.MagicScaler
{
	public static class WicImageProcessor
	{
		public static ProcessImageResult ProcessImage(string imgPath, Stream outStream, ProcessImageSettings settings)
		{
			using (var ctx = new WicProcessingContext(settings))
				return processImage(new WicDecoder(imgPath, ctx), ctx, outStream);
		}

		public static ProcessImageResult ProcessImage(ArraySegment<byte> imgBuffer, Stream outStream, ProcessImageSettings settings)
		{
			using (var ctx = new WicProcessingContext(settings))
				return processImage(new WicDecoder(imgBuffer, ctx), ctx, outStream);
		}

		public static ProcessImageResult ProcessImage(Stream imgStream, Stream outStream, ProcessImageSettings settings)
		{
			using (var ctx = new WicProcessingContext(settings))
				return processImage(new WicDecoder(imgStream, ctx), ctx, outStream);
		}

		private static ProcessImageResult processImage(WicDecoder dec, WicProcessingContext ctx, Stream ostm)
		{
			var frm = new WicFrameReader(ctx);
			WicTransforms.AddMetadataReader(ctx);

			ctx.FinalizeSettings();

			WicTransforms.AddNativeScaler(ctx);
			WicTransforms.AddExifRotator(ctx);
			WicTransforms.AddConditionalCache(ctx);
			WicTransforms.AddCropper(ctx);
			WicTransforms.AddPixelFormatConverter(ctx);
			WicTransforms.AddScaler(ctx);
			WicTransforms.AddColorspaceConverter(ctx);
			MagicTransforms.AddMatte(ctx);
			WicTransforms.AddIndexedColorConverter(ctx);

			var enc = new WicEncoder(ctx, ostm.AsIStream());
			enc.WriteSource(ctx);

			return new ProcessImageResult { Settings = ctx.UsedSettings, Stats = ctx.Stats };
		}
	}
}