#!/usr/bin/env bash
# cron:0 0 1 1 *
# new Env("bili尝试修复异常[dev先行版]")

dir_shell=$QL_DIR/shell
. $dir_shell/share.sh
. /root/.bashrc

bili_repo="raywangqvq/bilibilitoolpro"
bili_branch="_develop" 

# 目录
echo "青龙repo目录: $dir_repo"
# 仓库目录解析：1)脚本自身路径向上 2)当前目录向上 3)按名称查找 4)按项目标志扫描
# 兼容 ql otask/直接运行，以及任意 fork 账号与分支目录名；不再依赖写死的仓库名或 .git 位置
find_bili_repo_dir() {
    local d=$1
    while [ -n "$d" ] && [ "$d" != "/" ]; do
        if [ -f "$d/Ray.BiliBiliTool.sln" ]; then
            qinglong_bili_repo_dir="$d"
            return 0
        fi
        d="$(dirname "$d")"
    done
    return 1
}
qinglong_bili_repo_dir=""
if [ -n "${BASH_SOURCE[0]:-}" ]; then
    __bili_self="$(cd "$(dirname "${BASH_SOURCE[0]}")" 2>/dev/null && pwd || true)"
    find_bili_repo_dir "$__bili_self"
fi
if [ -z "$qinglong_bili_repo_dir" ]; then
    find_bili_repo_dir "$PWD"
fi
if [ -z "$qinglong_bili_repo_dir" ]; then
    qinglong_bili_repo="$(echo "$bili_repo" | sed 's/\//_/g')${bili_branch}"
    qinglong_bili_repo_dir="$(find $dir_repo -type d \( -iname $qinglong_bili_repo -o -iname ${qinglong_bili_repo}_main \) 2>/dev/null | head -1)"
fi
if [ -z "$qinglong_bili_repo_dir" ]; then
    qinglong_bili_repo_dir="$(find $dir_repo -maxdepth 1 -type d -exec test -f '{}/Ray.BiliBiliTool.sln' ';' -print 2>/dev/null | head -1)"
fi
if [ -z "$qinglong_bili_repo_dir" ]; then
    echo "bilitool: Warning: 未定位到 bili 仓库目录，请检查青龙中的仓库配置"
else
    echo "bili仓库目录: $qinglong_bili_repo_dir"
fi


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