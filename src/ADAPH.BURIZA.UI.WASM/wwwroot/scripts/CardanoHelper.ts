export function harden(num: number): number {
    return 0x80000000 + num;
}