import glob
import re

def full_null_fix():
    yaml_header = "%YAML 1.1\n%TAG !u! tag:unity3d.com,2011:\n"
    known_builtins = {0, 100100000, 11500000, 11400000, 2100000, 4800000, 4900000,
                      10304, 10001, 21300000, 23800000, 10905, 10907, 10202, 10206,
                      10207, 10208, 10209, 10913, 2180264, 20201}

    for s in glob.glob('Assets/Scenes/*.unity'):
        with open(s, 'r', encoding='utf-8') as f:
            text = f.read()

        blocks = re.findall(r'--- !u!(\d+) &(\d+)\n([\s\S]*?)(?=\n--- !u!|\Z)', text)
        all_fids = set([int(fid) for _, fid, _ in blocks])

        # Build set of all missing IDs
        missing_ids = set()
        for tid, fid, body in blocks:
            refs = re.findall(r'fileID: (\d+)', body)
            for r in refs:
                ir = int(r)
                if ir in known_builtins: continue
                if len(r) > 10: continue  # external ref
                if ir not in all_fids:
                    missing_ids.add(ir)

        if not missing_ids:
            print(f'{s}: PERFECT')
            # Still write back clean with yaml header
            repaired = yaml_header + '\n'.join([f'--- !u!{tid} &{fid}\n{body}' for tid, fid, body in blocks]) + '\n'
            with open(s, 'w', encoding='utf-8', newline='\n') as f:
                f.write(repaired)
            continue

        print(f'{s}: Nulling {len(missing_ids)} broken references...')

        # Replace all broken fileID references with fileID: 0
        repaired_blocks = []
        for tid, fid, body in blocks:
            for mid in missing_ids:
                # Replace any reference to missing ID with 0
                body = re.sub(rf'\{{fileID: {mid}\}}', '{fileID: 0}', body)
            repaired_blocks.append(f'--- !u!{tid} &{fid}\n{body}')

        full_text = yaml_header + '\n'.join(repaired_blocks) + '\n'
        with open(s, 'w', encoding='utf-8', newline='\n') as f:
            f.write(full_text)
        print(f'  Done.')

if __name__ == '__main__':
    full_null_fix()
