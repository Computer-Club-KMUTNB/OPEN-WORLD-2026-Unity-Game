import glob
import re

def find_all_broken_refs():
    yaml_header = "%YAML 1.1\n%TAG !u! tag:unity3d.com,2011:\n"
    known_builtins = {0, 100100000, 11500000, 11400000, 2100000, 4800000, 4900000,
                      10304, 10001, 21300000, 23800000, 10905, 10907, 10202, 10206,
                      10207, 10208, 10209, 10913, 2180264, 20201}

    for s in glob.glob('Assets/Scenes/*.unity'):
        with open(s, 'r', encoding='utf-8') as f:
            text = f.read()

        blocks = re.findall(r'--- !u!(\d+) &(\d+)\n([\s\S]*?)(?=\n--- !u!|\Z)', text)
        all_fids = set([int(fid) for _, fid, _ in blocks])

        bad_refs = {}
        for tid, fid, body in blocks:
            refs = re.findall(r'fileID: (\d+)', body)
            for r in refs:
                ir = int(r)
                if ir in known_builtins: continue
                if len(r) > 10: continue  # external guid-based ref
                if ir not in all_fids:
                    bad_refs.setdefault(ir, []).append((tid, fid))

        if bad_refs:
            print(f'\n[{s}] {len(bad_refs)} broken internal IDs:')
            for bid, owners in sorted(bad_refs.items()):
                print(f'  Missing ID {bid} referenced in blocks: {owners[:3]}')
        else:
            print(f'[{s}]: PERFECT')

if __name__ == '__main__':
    find_all_broken_refs()
