# SaveOurShip Godot
## 如何提交代码
- master分支是我们的成品分支，一般来说不会修改它的内容。绝大多数的日常更新在dev分支上进行。
- 如果你完成了一个功能，极度推荐创建一个PR(Pull Request)。PR能够很好地记录你的工作量，并方便排查问题等。
### 创建PR的过程
1. 创建一个你自己的branch并切换到它。
   ```
   git branch -b combat
   ```
   一般而言，要做什么就起什么名字，例如编写了战斗就取名为`combat`。
   你可以在上面做任何事情，不用担心弄坏别人写到一半的代码。

4. 进行你的更改，并commit在上面。
   
   如果你在编写的时候不小心正处于其他的branch上（比如master或dev），你可以暂存更改，切换到正确的branch之后，再把暂存区的更改commit到上面。
   ```
   # 先保存你刚做好的修改
   git add .
   # 现在你可以安全切换分支（例如combat）
   git checkout combat      # 假如还没创建combat分支，就用上面的命令（git branch -b）去创建它
   # 然后进行commit
   git commit -m "lorem ipsum"
   ```
5. 现在你可以创建PR了。

   * 打开github。切换到Pull request页（实际上如果你刚commit完它会给你一个显眼的提示，用来一键创建PR）；
   * 选择base为`dev`；
   * 选择compare为你自己的分支（例如`combat`）。
   * 点击创建。往里面稍微写点有意义的内容，例如你刚做了什么，这样其他人可以很容易复核并讨论，假如存在问题的话。
  
PR是一个很有用的工具。如果你只做了一半，也可以创建PR并就此询问其他人，等全部完成后再合并。

在PR被确认合并到dev分支后，你就可以安全删除你先前使用的那个分支了（例如combat）。如果你要对这项功能进行长期更新，也可以保留这个分支不删除。

### 关于分支

不要害怕创建新的分支，大量的分支并不会带来坏处，反而会让事情更清晰。此外，一个commit（和PR）最好只包括一件事。如果你解决了多个问题，最好创建多个commit（和PR）——这样还可以显得你做了很多工作，这是好事。

最好在你做完一件小事后就进行commit，然后勤加push到远程仓库（github）上。
```
git push
```

### 关于旧分支的工作准备
如果你的分支是刚创建的，你可以不用做这些事情。

如果你担心你的分支上，可能有一些你会用到的代码不是最新的，就进行这样的操作：
1. 切换到你的分支（比如combat）；
   ```
   git checkout combat
   ```
2. （可选）更新你本地关于dev的最新更改；
   最好这么做，因为你的本地仓库可能不是最新的。
   ```
   git pull origin dev
   ```
3. 从dev将其他人（也可能是你自己）的更新，合并到你的目标分支上；
   ```
   git merge dev
   ```
4. （可能发生）如果有冲突，尝试解决它。可以用AI或者来问我。或者最简单的方法，重新根据dev分支创建一个新的分支。
5. 如果你需要的话，推送到远程。
   ```
   git push origin combat
   ```
