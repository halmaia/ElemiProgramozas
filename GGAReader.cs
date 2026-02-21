 const byte fieldSeparator = (byte)',';
 ReadOnlySpan<byte> buffer = "$GPGGA,123519.00,4807.038,N,01131.000,E,1,08,0.9,545.4,M,-164.0,M,,*47"u8;
 MemoryExtensions.SpanSplitEnumerator<byte> fields = buffer.Split(fieldSeparator);
 if (!fields.MoveNext()) return;
 ReadOnlySpan<byte> field = buffer[fields.Current];
 if (field[0] is not (byte)'$' || !field.EndsWith("GGA"u8)) return;
 
 if (!fields.MoveNext()) return;
 field = buffer[fields.Current];
 if (field.Length < 6 ||
     !Utf8Parser.TryParse(field.Slice(0, 2), out byte hour, out int bytesConsumed) ||
     bytesConsumed is not 2 ||
     !Utf8Parser.TryParse(field.Slice(2, 2), out byte minute, out bytesConsumed) ||
     bytesConsumed is not 2 ||
     !Utf8Parser.TryParse(field.Slice(4), out double second, out bytesConsumed) ||
     bytesConsumed < 2) return;
 TimeOnly utcTime = new(TimeSpan.TicksPerHour * hour + TimeSpan.TicksPerMinute * minute + double.ConvertToIntegerNative<long>(double.FusedMultiplyAdd(TimeSpan.TicksPerSecond, second, .5d)));
 
 if (!fields.MoveNext()) return;
 field = buffer[fields.Current];
 if (!Utf8Parser.TryParse(field.Slice(0, 2), out byte latDeg, out bytesConsumed) ||
     bytesConsumed is not 2 ||
     !Utf8Parser.TryParse(field.Slice(2), out double latMin, out bytesConsumed) ||
     bytesConsumed < 2) return;
 double lat = double.FusedMultiplyAdd(1d / 60d, latMin, latDeg);
 
 if (!fields.MoveNext()) return;
 field = buffer[fields.Current];
 if (field.Length is not 1) return;
 switch (field[0])
 {
     case (byte)'E':
         lat = -lat;
         break;
     case not (byte)'N':
         return;
 }
 
 // Not ready...
