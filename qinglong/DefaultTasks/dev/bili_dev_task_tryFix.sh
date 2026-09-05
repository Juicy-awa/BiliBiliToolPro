#!/usr/bin/env bash
# cron:0 0 1 1 *
# new Env("bili尝试修复异常[dev先行版]")

dir_shell=$QL_DIR/shell
. $dir_shell/share.sh
. /root/.bashrc

bili_repo="raywangqvq/bilibilitoolpro"
bili_branch="_develop" 

echo "青龙repo目录: $dir_repo"
# 优先解析脚本自身所在仓库根目录（兼容任意 fork/账号/分支目录名）
# 无法定位时回退为按 bili_repo 名称在 repo 目录中查找，兼容旧部署
__bili_self_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" >/dev/null 2>&1 && pwd)"
qinglong_bili_repo_dir=""
while [ -n "$__bili_self_dir" ] && [ "$__bili_self_dir" != "/" ]; do
    if [ -d "$__bili_self_dir/.git" ]; then
        qinglong_bili_repo_dir="$__bili_self_dir"
        break
    fi
    __bili_self_dir="$(dirname "$__bili_self_dir")"
done
if [ -z "$qinglong_bili_repo_dir" ]; then
    qinglong_bili_repo="$(echo "$bili_repo" | sed 's/\//_/g')${bili_branch}"
    qinglong_bili_repo_dir="$(find $dir_repo -type d \( -iname $qinglong_bili_repo -o -iname ${qinglong_bili_repo}_main \) | head -1)"
fi
echo "bili仓库目录: $qinglong_bili_repo_dir"


echo -e "清理缓存...\n"
cd $qinglong_bili_repo_dir
find . -type d -name "bin" -exec rm -rf {} +
find . -type d -name "obj" -exec rm -rf {} +
echo -e "清理完成\n"

echo "检测dotnet..."
dotnetVersion=$(dotnet --version)
echo "当前dotnet版本：$dotnetVersion"
if [[ $(echo "$dotnetVersion" | grep -oE '^[0-9]+') -ge 8 ]]; then
    echo "已安装，且版本满足"
else
    echo "which dotnet: $(which dotnet)"
    echo "Path: $PATH"
    rm -f /usr/local/bin/dotnet
fi
echo "检测dotnet结束"