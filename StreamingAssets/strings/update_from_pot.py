import sys
import subprocess
import re

potfile = sys.argv[1]
print ('Potfile:', potfile)

pofiles = [
    'strings_preinstalled_ko_klei.po',
    'strings_preinstalled_ru_klei.po',
    'strings_preinstalled_zh_klei.po'
]

revision_match = re.search(r"_r(\d*)", potfile)
revision = 0
if revision_match is not None:
    revision = int(revision_match.group(1))

print ("Found revision", revision)

for po in pofiles:
    print("Processing", po)
    root = po.split('.')[0]
    command = ['msgmerge.exe', '-o', '{0}_{1}.po'.format(root, revision), '-N', '--no-wrap', po, potfile]
    print(' '.join(command))
    subprocess.call(command)

print ("Done.")
