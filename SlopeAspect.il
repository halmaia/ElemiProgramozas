.method private hidebysig static 
	void GetSlopeAndAspect (
		float64[] matrix,
		int32 stride,
		float64 cellsize,
		[out] float64[]& slope,
		[out] float64[]& aspect
	) cil managed 
{
	// Method begins at RVA 0x207c
	// Code size 491 (0x1eb)
	.maxstack 6
	.locals init (
		[0] int32 matrix_length,
		[1] int32 output_length,
		[2] int32 n,
		[3] float64 divider,
		[4] int32 b,
		[5] int32 a,
		[6] int32 c,
		[7] int32 g,
		[8] int32 i,
		[9] int32 y,
		[10] int32 x,
		[11] int32 index,
		[12] float64 val_i,
		[13] float64 val_a,
		[14] float64 val_c,
		[15] float64 val_g,
		[16] float64 dx,
		[17] float64 dy,
		[18] float64 imed_aspect
	)

	IL_0000: ldarg.0
	IL_0001: brtrue.s IL_000e

	IL_0003: ldstr "matrix"
	IL_0008: newobj instance void [mscorlib]System.ArgumentNullException::.ctor(string)
	IL_000d: throw

	IL_000e: ldarg.0
	IL_000f: ldlen
	IL_0010: conv.i4
	IL_0011: stloc.0
	IL_0012: ldloc.0
	IL_0013: ldc.i4.s 9
	IL_0015: bge.s IL_0022

	IL_0017: ldstr "matrix"
	IL_001c: newobj instance void [mscorlib]System.ArgumentOutOfRangeException::.ctor(string)
	IL_0021: throw

	IL_0022: ldarg.1
	IL_0023: ldc.i4.3
	IL_0024: blt.s IL_002f

	IL_0026: ldarg.1
	IL_0027: ldloc.0
	IL_0028: bge.s IL_002f

	IL_002a: ldloc.0
	IL_002b: ldarg.1
	IL_002c: rem
	IL_002d: brfalse.s IL_003a

	IL_002f: ldstr "stride"
	IL_0034: newobj instance void [mscorlib]System.ArgumentOutOfRangeException::.ctor(string)
	IL_0039: throw

	IL_003a: ldarg.2
	IL_003b: call bool [mscorlib]System.Double::IsNaN(float64)
	IL_0040: brtrue.s IL_004e

	IL_0042: ldarg.2
	IL_0043: ldc.r8 0.0
	IL_004c: bgt.un.s IL_0059

	IL_004e: ldstr "cellsize"
	IL_0053: newobj instance void [mscorlib]System.ArgumentOutOfRangeException::.ctor(string)
	IL_0058: throw

	IL_0059: ldloc.0
	IL_005a: ldarg.1
	IL_005b: div
	IL_005c: ldc.i4.2
	IL_005d: sub
	IL_005e: ldarg.1
	IL_005f: ldc.i4.2
	IL_0060: sub
	IL_0061: mul
	IL_0062: stloc.1
	IL_0063: ldarg.3
	IL_0064: ldloc.1
	IL_0065: newarr [mscorlib]System.Double
	IL_006a: stind.ref
	IL_006b: ldarg.s aspect
	IL_006d: ldloc.1
	IL_006e: newarr [mscorlib]System.Double
	IL_0073: stind.ref
	IL_0074: ldc.i4.0
	IL_0075: stloc.2
	IL_0076: ldc.r8 8
	IL_007f: ldarg.2
	IL_0080: mul
	IL_0081: stloc.3
	IL_0082: ldarg.1
	IL_0083: neg
	IL_0084: stloc.s 4
	IL_0086: ldloc.s 4
	IL_0088: ldc.i4.1
	IL_0089: sub
	IL_008a: stloc.s 5
	IL_008c: ldloc.s 4
	IL_008e: ldc.i4.1
	IL_008f: add
	IL_0090: stloc.s 6
	IL_0092: ldarg.1
	IL_0093: ldc.i4.1
	IL_0094: sub
	IL_0095: stloc.s 7
	IL_0097: ldarg.1
	IL_0098: ldc.i4.1
	IL_0099: add
	IL_009a: stloc.s 8
	IL_009c: ldarg.1
	IL_009d: stloc.s 9
	IL_009f: br IL_01e0
	// loop start (head: IL_01e0)
		IL_00a4: ldc.i4.1
		IL_00a5: stloc.s 10
		IL_00a7: br IL_01d1
		// loop start (head: IL_01d1)
			IL_00ac: ldloc.s 10
			IL_00ae: ldloc.s 9
			IL_00b0: add
			IL_00b1: stloc.s 11
			IL_00b3: ldarg.0
			IL_00b4: ldloc.s 11
			IL_00b6: ldloc.s 8
			IL_00b8: add
			IL_00b9: ldelem.r8
			IL_00ba: stloc.s 12
			IL_00bc: ldarg.0
			IL_00bd: ldloc.s 11
			IL_00bf: ldloc.s 5
			IL_00c1: add
			IL_00c2: ldelem.r8
			IL_00c3: stloc.s 13
			IL_00c5: ldarg.0
			IL_00c6: ldloc.s 11
			IL_00c8: ldloc.s 6
			IL_00ca: add
			IL_00cb: ldelem.r8
			IL_00cc: stloc.s 14
			IL_00ce: ldarg.0
			IL_00cf: ldloc.s 11
			IL_00d1: ldloc.s 7
			IL_00d3: add
			IL_00d4: ldelem.r8
			IL_00d5: stloc.s 15
			IL_00d7: ldloc.s 14
			IL_00d9: ldc.r8 2
			IL_00e2: ldarg.0
			IL_00e3: ldloc.s 11
			IL_00e5: ldc.i4.1
			IL_00e6: add
			IL_00e7: ldelem.r8
			IL_00e8: mul
			IL_00e9: add
			IL_00ea: ldloc.s 12
			IL_00ec: add
			IL_00ed: ldloc.s 13
			IL_00ef: ldc.r8 2
			IL_00f8: ldarg.0
			IL_00f9: ldloc.s 11
			IL_00fb: ldc.i4.1
			IL_00fc: sub
			IL_00fd: ldelem.r8
			IL_00fe: mul
			IL_00ff: add
			IL_0100: ldloc.s 15
			IL_0102: add
			IL_0103: sub
			IL_0104: ldloc.3
			IL_0105: div
			IL_0106: stloc.s 16
			IL_0108: ldloc.s 15
			IL_010a: ldc.r8 2
			IL_0113: ldarg.0
			IL_0114: ldloc.s 11
			IL_0116: ldarg.1
			IL_0117: add
			IL_0118: ldelem.r8
			IL_0119: mul
			IL_011a: add
			IL_011b: ldloc.s 12
			IL_011d: add
			IL_011e: ldloc.s 13
			IL_0120: ldc.r8 2
			IL_0129: ldarg.0
			IL_012a: ldloc.s 11
			IL_012c: ldarg.1
			IL_012d: sub
			IL_012e: ldelem.r8
			IL_012f: mul
			IL_0130: add
			IL_0131: ldloc.s 14
			IL_0133: add
			IL_0134: sub
			IL_0135: ldloc.3
			IL_0136: div
			IL_0137: stloc.s 17
			IL_0139: ldarg.3
			IL_013a: ldind.ref
			IL_013b: ldloc.2
			IL_013c: ldloc.s 17
			IL_013e: ldloc.s 17
			IL_0140: mul
			IL_0141: ldloc.s 16
			IL_0143: ldloc.s 16
			IL_0145: mul
			IL_0146: add
			IL_0147: call float64 [mscorlib]System.Math::Sqrt(float64)
			IL_014c: stelem.r8
			IL_014d: ldc.r8 0.0
			IL_0156: stloc.s 18
			IL_0158: ldloc.s 16
			IL_015a: ldc.r8 0.0
			IL_0163: beq.s IL_018e

			IL_0165: ldloc.s 17
			IL_0167: ldloc.s 16
			IL_0169: neg
			IL_016a: call float64 [mscorlib]System.Math::Atan2(float64, float64)
			IL_016f: stloc.s 18
			IL_0171: ldloc.s 18
			IL_0173: ldc.r8 0.0
			IL_017c: bge.un.s IL_01c0

			IL_017e: ldloc.s 18
			IL_0180: ldc.r8 6.2831853071795862
			IL_0189: add
			IL_018a: stloc.s 18
			IL_018c: br.s IL_01c0

			IL_018e: ldloc.s 17
			IL_0190: ldc.r8 0.0
			IL_0199: ble.un.s IL_01a8

			IL_019b: ldc.r8 1.5707963267948966
			IL_01a4: stloc.s 18
			IL_01a6: br.s IL_01c0

			IL_01a8: ldloc.s 17
			IL_01aa: ldc.r8 0.0
			IL_01b3: bge.un.s IL_01c0

			IL_01b5: ldc.r8 4.71238898038469
			IL_01be: stloc.s 18

			IL_01c0: ldarg.s aspect
			IL_01c2: ldind.ref
			IL_01c3: ldloc.2
			IL_01c4: dup
			IL_01c5: ldc.i4.1
			IL_01c6: add
			IL_01c7: stloc.2
			IL_01c8: ldloc.s 18
			IL_01ca: stelem.r8
			IL_01cb: ldloc.s 10
			IL_01cd: ldc.i4.1
			IL_01ce: add
			IL_01cf: stloc.s 10

			IL_01d1: ldloc.s 10
			IL_01d3: ldloc.s 7
			IL_01d5: blt IL_00ac
		// end loop

		IL_01da: ldloc.s 9
		IL_01dc: ldarg.1
		IL_01dd: add
		IL_01de: stloc.s 9

		IL_01e0: ldloc.s 9
		IL_01e2: ldloc.0
		IL_01e3: ldarg.1
		IL_01e4: sub
		IL_01e5: blt IL_00a4
	// end loop

	IL_01ea: ret
} // end of method Program::GetSlopeAndAspect
