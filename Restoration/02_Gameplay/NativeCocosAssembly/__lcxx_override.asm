; Full executable section __lcxx_override; VA 0x1a7d444; 268 bytes

; FUNCTION _Znwm
0000000001a7d444: 3f2303d5     paciasp   
0000000001a7d448: fd7bbea9     stp       x29, x30, [sp, #-0x20]!
0000000001a7d44c: f30b00f9     str       x19, [sp, #0x10]
0000000001a7d450: fd030091     mov       x29, sp
0000000001a7d454: 1f0400f1     cmp       x0, #1
0000000001a7d458: 13849f9a     csinc     x19, x0, xzr, hi
0000000001a7d45c: e00313aa     mov       x0, x19
0000000001a7d460: 34020094     bl        #0x1a7dd30
0000000001a7d464: a00000b5     cbnz      x0, #0x1a7d478
0000000001a7d468: 52580194     bl        #0x1ad35b0
0000000001a7d46c: e00000b4     cbz       x0, #0x1a7d488
0000000001a7d470: 00003fd6     blr       x0
0000000001a7d474: faffff17     b         #0x1a7d45c
0000000001a7d478: f30b40f9     ldr       x19, [sp, #0x10]
0000000001a7d47c: fd7bc2a8     ldp       x29, x30, [sp], #0x20
0000000001a7d480: bf2303d5     autiasp   
0000000001a7d484: c0035fd6     ret       
0000000001a7d488: 00018052     mov       w0, #8
0000000001a7d48c: 69010094     bl        #0x1a7da30
0000000001a7d490: f30300aa     mov       x19, x0
0000000001a7d494: 7f550194     bl        #0x1ad2a90
0000000001a7d498: 810700f0     adrp      x1, #0x1b70000
0000000001a7d49c: 820700f0     adrp      x2, #0x1b70000
0000000001a7d4a0: e00313aa     mov       x0, x19
0000000001a7d4a4: 21d440f9     ldr       x1, [x1, #0x1a8]
0000000001a7d4a8: 42d840f9     ldr       x2, [x2, #0x1b0]
0000000001a7d4ac: 69010094     bl        #0x1a7da50

; FUNCTION _Znam
0000000001a7d4b0: 5f2403d5     bti       c
0000000001a7d4b4: 4b000014     b         #0x1a7d5e0

; FUNCTION _ZnwmSt11align_val_t
0000000001a7d4b8: 3f2303d5     paciasp   
0000000001a7d4bc: ffc300d1     sub       sp, sp, #0x30
0000000001a7d4c0: fd7b01a9     stp       x29, x30, [sp, #0x10]
0000000001a7d4c4: f44f02a9     stp       x20, x19, [sp, #0x20]
0000000001a7d4c8: fd430091     add       x29, sp, #0x10
0000000001a7d4cc: 1f0400f1     cmp       x0, #1
0000000001a7d4d0: 08018052     mov       w8, #8
0000000001a7d4d4: 13849f9a     csinc     x19, x0, xzr, hi
0000000001a7d4d8: 3f2000f1     cmp       x1, #8
0000000001a7d4dc: 3480889a     csel      x20, x1, x8, hi
0000000001a7d4e0: e0230091     add       x0, sp, #8
0000000001a7d4e4: e10314aa     mov       x1, x20
0000000001a7d4e8: e20313aa     mov       x2, x19
0000000001a7d4ec: ff0700f9     str       xzr, [sp, #8]
0000000001a7d4f0: 04550194     bl        #0x1ad2900
0000000001a7d4f4: e00740f9     ldr       x0, [sp, #8]
0000000001a7d4f8: a00000b5     cbnz      x0, #0x1a7d50c
0000000001a7d4fc: 2d580194     bl        #0x1ad35b0
0000000001a7d500: 000100b4     cbz       x0, #0x1a7d520
0000000001a7d504: 00003fd6     blr       x0
0000000001a7d508: f6ffff17     b         #0x1a7d4e0
0000000001a7d50c: f44f42a9     ldp       x20, x19, [sp, #0x20]
0000000001a7d510: fd7b41a9     ldp       x29, x30, [sp, #0x10]
0000000001a7d514: ffc30091     add       sp, sp, #0x30
0000000001a7d518: bf2303d5     autiasp   
0000000001a7d51c: c0035fd6     ret       
0000000001a7d520: 00018052     mov       w0, #8
0000000001a7d524: 43010094     bl        #0x1a7da30
0000000001a7d528: f30300aa     mov       x19, x0
0000000001a7d52c: 59550194     bl        #0x1ad2a90
0000000001a7d530: 810700f0     adrp      x1, #0x1b70000
0000000001a7d534: 820700f0     adrp      x2, #0x1b70000
0000000001a7d538: e00313aa     mov       x0, x19
0000000001a7d53c: 21d440f9     ldr       x1, [x1, #0x1a8]
0000000001a7d540: 42d840f9     ldr       x2, [x2, #0x1b0]
0000000001a7d544: 43010094     bl        #0x1a7da50

; FUNCTION _ZnamSt11align_val_t
0000000001a7d548: 5f2403d5     bti       c
0000000001a7d54c: 41580114     b         #0x1ad3650
